namespace Common.Models.Events.Tasks.Streaming;

public class TaskStreamingUpdatedPriceV1 : EventBase<TaskStreamingUpdatedPriceDataV1>
{
    public TaskStreamingUpdatedPriceV1(TaskStreamingUpdatedPriceDataV1 data)
        : base(TasksStreamConstants.EventNames.TaskPriceUpdated, 1, data)
    {
    }
}

public class TaskStreamingUpdatedPriceDataV1
{
    public Guid TaskId { get; init; }
    public decimal AssignPrice { get; init; }
    public decimal CompletePrice { get; init; }
}