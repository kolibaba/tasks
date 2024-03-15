using analyticsService.Models;
using analyticsService.Repositories;
using Common.BrokerMessage;
using Common.Models.Events.Billing;
using Common.Models.Events.Tasks.Streaming;

namespace analyticsService;

public static class AnalyticsInitializer
{
    public static void Init()
    {
        Task.Run(ConsumeEvents);
    }

    private static void ConsumeEvents()
    {
        var analyticsTasksRepository = AnalyticsTasksRepository.Instance;

        //tasks stream
        BrokerMessageConsumer.Consume<TaskStreamingCreatedV1>(
            TasksStreamConstants.TopicName,
            TasksStreamConstants.EventNames.TaskCreated,
            "tasks/streaming_task_created",
            1,
            eventDto =>
            {
                analyticsTasksRepository.Add(
                    new AnalyticsTask
                    {
                        TaskId = eventDto.Data.TaskId,
                        TaskText = eventDto.Data.TaskText,
                        CreatedAt = eventDto.Data.CreatedAt
                    });
            });

        //tasks price stream
        BrokerMessageConsumer.Consume<TaskStreamingUpdatedPriceV1>(
            TasksStreamConstants.TopicName,
            TasksStreamConstants.EventNames.TaskPriceUpdated,
            "tasks/streaming_task_price_updated",
            1,
            eventDto =>
            {
                analyticsTasksRepository.UpdatePrices(
                    eventDto.Data.TaskId,
                    eventDto.Data.AssignPrice,
                    eventDto.Data.CompletePrice);
            });

        BrokerMessageConsumer.Consume<TransactionAddedV1>(
            "billing",
            EventsNames.TransactionAdded,
            "billing/transaction_added",
            1,
            eventDto =>
            {
                AnalyticsTransactionsRepository.Instance.Add(new AnalyticsTransaction
                {
                    UserId = eventDto.Data.UserId,
                    TransactionType = eventDto.Data.TransactionType,
                    Value = eventDto.Data.Value,
                    Text = eventDto.Data.Text
                    //add props?
                });
            });
    }
}