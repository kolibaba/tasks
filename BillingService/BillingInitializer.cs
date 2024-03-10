using Common.BrokerMessage;
using Common.Models;
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
        BrokerMessageConsumer.Consume<UsersStream.UserCreated>(UsersStream.TopicName, UsersStream.UserCreated.EventName,
            eventDto =>
            {
                BillingUsersRepository.Instance.Add(new BillingUser
                {
                    UserId = eventDto.UserId,
                    Role = eventDto.Role,
                    Balance = 0
                });
            });

        //tasks stream
        BrokerMessageConsumer.Consume<TasksStream.TaskCreated>(TasksStream.TopicName, TasksStream.TaskCreated.EventName,
            eventDto =>
            {
                var assignPrice = random.Next(10, 21);
                var completePrice = random.Next(20, 31);
                BillingTaskRepository.Instance.Add(
                    new BillingTask
                    {
                        TaskId = eventDto.TaskId,
                        TaskText = eventDto.TaskText,
                        AssignPrice = assignPrice,
                        CompletePrice = completePrice
                    });
            });

        //task assigned BE
        BrokerMessageConsumer.Consume<TasksEvents.TaskAssigned>(TasksEvents.TopicName,
            TasksEvents.TaskAssigned.EventName,
            eventDto =>
            {
                var taskId = eventDto.TaskId;
                var task = GetTask(taskId);
                TransactionsRepository.Instance.Add(eventDto.AssigneeUserId, TransactionType.Credit, task.AssignPrice,
                    $"assigned task {taskId}");
            });

        //task multiple assigned (shuffled) BE
        BrokerMessageConsumer.Consume<TasksEvents.TasksShuffled>(TasksEvents.TopicName,
            TasksEvents.TasksShuffled.EventName,
            eventDto =>
            {
                foreach (var eventItem in eventDto.List)
                {
                    var taskId = eventItem.TaskId;
                    var task = GetTask(taskId);
                    TransactionsRepository.Instance.Add(eventItem.AssigneeUserId, TransactionType.Credit,
                        task.AssignPrice, $"assigned task {taskId}");
                }
            });

        //task completed BE
        BrokerMessageConsumer.Consume<TasksEvents.TaskCompleted>(TasksEvents.TopicName,
            TasksEvents.TaskCompleted.EventName,
            eventDto =>
            {
                var taskId = eventDto.TaskId;
                var task = GetTask(taskId);
                TransactionsRepository.Instance.Add(eventDto.ByUserId, TransactionType.Debit, task.CompletePrice,
                    $"completed task {taskId}");
            });
    }

    private static BillingTask GetTask(Guid taskId)
    {
        var task = BillingTaskRepository.Instance.Find(taskId);
        if (task == null) throw new InvalidOperationException($"Couldn't find task with id={taskId}");

        return task;
    }
}