using Common.BrokerMessage;
using Common.Models;
using Common.Models.Events;
using Common.Models.Events.Accouns.Streaming;
using Common.Models.Events.Billing;
using Common.Models.Events.Tasks;
using Common.Models.Events.Tasks.Streaming;
using tasksManager.Models;
using tasksManager.Repositories;

namespace tasksManager;

public static class BillingInitializer
{
    private static readonly Random random = new();

    public static void Init()
    {
        Task.Run(ConsumeEvents);
    }

    private static void ConsumeEvents()
    {
        //users stream
        BrokerMessageConsumer.Consume<AccountStreamingCreatedV1>(
            AccountStreamingConstants.TopicName,
            AccountStreamingConstants.EventNames.AccountCreated,
            "accounts/streaming_account_created",
            1,
            eventDto =>
            {
                BillingUsersRepository.Instance.Add(new BillingUser
                {
                    UserId = eventDto.Data.UserId,
                    Role = eventDto.Data.Role,
                    Balance = 0
                });
            });

        //tasks stream
        BrokerMessageConsumer.ConsumeAsync<TaskStreamingCreatedV1>(
            TasksStreamConstants.TopicName,
            TasksStreamConstants.EventNames.TaskCreated,
            "tasks/streaming_task_created",
            1,
            async eventDto =>
            {
                var assignPrice = random.Next(10, 21);
                var completePrice = random.Next(20, 31);
                await BrokerMessageProducer.Produce(
                    TasksStreamConstants.TopicName,
                    "tasks/streaming_task_price_updated",
                    new TaskStreamingUpdatedPriceV1(
                        new TaskStreamingUpdatedPriceDataV1
                    {
                        AssignPrice = assignPrice,
                        CompletePrice = completePrice
                    }));
                
                BillingTasksRepository.Instance.Add(
                    new BillingTask
                    {
                        TaskId = eventDto.Data.TaskId,
                        TaskText = eventDto.Data.TaskText,
                        AssignPrice = assignPrice,
                        CompletePrice = completePrice
                    });
            });

        //task created BE
        BrokerMessageConsumer.ConsumeAsync<TaskAssignedV1>(
            TasksEventsConstants.TopicName,
            TasksEventsConstants.TaskCreated,
            "tasks/task_created",
            1,
            async eventDto =>
            {
                var taskId = eventDto.Data.TaskId;
                var task = GetTask(taskId);
                var assigneeUserId = eventDto.Data.AssigneeUserId;
                var assignPrice = task.AssignPrice;
                //add transaction 
                var billingCycle = BillingCyclesRepository.Instance.GetOrCreateCurrent();
                var transaction = new Transaction
                {
                    TransactionType = TransactionType.Credit,
                    Value = assignPrice,
                    Text = $"assigned task {taskId}",
                    BillingCycleId = billingCycle.Id
                };
                await BrokerMessageProducer.Produce("billing", "billing/transaction_added", 
                    new TransactionAddedV1(new TransactionAddedV1Data
                {
                    TransactionType = transaction.TransactionType,
                    Value = transaction.Value,
                    UserId = assigneeUserId,
                    Text = transaction.Text
                }));
                TransactionsRepository.Instance.Add(assigneeUserId, transaction);
                
                var user = BillingUsersRepository.Instance.GetUser(assigneeUserId);
                user.Balance -= assignPrice;
            });

        //task multiple assigned (shuffled) BE
        BrokerMessageConsumer.ConsumeAsync<TasksShuffledV1>(
            TasksEventsConstants.TopicName,
            TasksEventsConstants.TasksShuffled,
            "tasks/tasks_shuffled",
            1,
            async eventDto =>
            {
                foreach (var eventItem in eventDto.Data.List)
                {
                    var taskId = eventItem.TaskId;
                    var task = GetTask(taskId);
                    var assigneeUserId = eventItem.AssigneeUserId;
                    var assignPrice = task.AssignPrice;
                    //add trans
                    var billingCycle = BillingCyclesRepository.Instance.GetOrCreateCurrent();
                    var transaction = new Transaction
                    {
                        TransactionType = TransactionType.Credit,
                        Value = assignPrice,
                        Text = $"assigned task {taskId}",
                        BillingCycleId = billingCycle.Id
                    };
                    TransactionsRepository.Instance.Add(assigneeUserId, transaction);
                    await BrokerMessageProducer.Produce("billing", "billing/transaction_added", 
                        new TransactionAddedV1(new TransactionAddedV1Data
                        {
                            TransactionType = transaction.TransactionType,
                            Value = transaction.Value,
                            UserId = assigneeUserId,
                            Text = transaction.Text
                        }));
                    
                    var user = BillingUsersRepository.Instance.GetUser(assigneeUserId);
                    user.Balance -= assignPrice;
                }
            });

        //task completed BE
        BrokerMessageConsumer.Consume<TaskCompletedV1>(
            TasksEventsConstants.TopicName,
            TasksEventsConstants.TaskCompleted,
            "tasks/task_completed",
            1,
            eventDto =>
            {
                var taskId = eventDto.Data.TaskId;
                var task = GetTask(taskId);
                var userId = eventDto.Data.ByUserId;
                var completePrice = task.CompletePrice;
                //add trans
                var billingCycle = BillingCyclesRepository.Instance.GetOrCreateCurrent();
                var transaction = new Transaction
                {
                    TransactionType = TransactionType.Debit,
                    Value = completePrice,
                    Text = $"completed task {taskId}",
                    BillingCycleId = billingCycle.Id
                };
                TransactionsRepository.Instance.Add(userId, transaction);

                var user = BillingUsersRepository.Instance.GetUser(userId);
                user.Balance -= completePrice;
            });
    }

    private static BillingTask GetTask(Guid taskId)
    {
        var task = BillingTasksRepository.Instance.Find(taskId);
        if (task == null) throw new InvalidOperationException($"Couldn't find task with id={taskId}");
        return task;
    }
}