namespace Common.Models.Events.Tasks;

public class TaskAssignedV1 : EventBase<TaskAssignedData>
{
    public TaskAssignedV1(TaskAssignedData data) : base(EventsNames.TaskAssigned, 1, data)
    {
    }
}

public record TaskAssignedData
{
    public Guid TaskId { get; set; }
    public Guid AssigneeUserId { get; set; }
}