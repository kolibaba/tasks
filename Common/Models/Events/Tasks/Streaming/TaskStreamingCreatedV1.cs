namespace Common.Models.Events.Tasks.Streaming;

public class TaskStreamingCreatedV1 : EventBase<TaskStreamCreatedDataV1>
{
    public TaskStreamingCreatedV1(TaskStreamCreatedDataV1 data)
        : base(TasksStreamConstants.EventNames.TaskCreated, 1, data)
    {
    }
}

public class TaskStreamCreatedDataV1
{
    public Guid TaskId { get; set; }
    public string TaskText { get; set; }
    public DateTime CreatedAt { get; set; }
}