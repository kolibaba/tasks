namespace Common.Models.Events.Tasks;

public class TaskCreatedV1 : EventBase<TaskCreatedDataV1>
{
    public TaskCreatedV1(TaskCreatedDataV1 data) : base(EventsNames.TaskCreated, 1, data)
    {
    }
}

public class TaskCreatedDataV1
{
    public Guid TaskId { get; set; }
    public Guid AssigneeUserId { get; set; }
}