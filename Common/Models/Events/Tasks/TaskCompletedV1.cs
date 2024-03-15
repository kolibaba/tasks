namespace Common.Models.Events.Tasks;

public class TaskCompletedV1 : EventBase<TaskCompletedDataV1>
{
    public TaskCompletedV1(TaskCompletedDataV1 data) : base(EventsNames.TaskCompleted, 1, data)
    {
    }
}

public class TaskCompletedDataV1
{
    public Guid TaskId { get; set; }
    public Guid ByUserId { get; set; }
}