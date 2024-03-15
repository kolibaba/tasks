namespace Common.Models.Events.Tasks;

public class TasksShuffledV1 : EventBase<TasksShuffledDataV1>
{
    public TasksShuffledV1(TasksShuffledDataV1 data) : base(EventsNames.TasksShuffled, 1, data)
    {
    }
}

public class TasksShuffledDataV1
{
    public List<(Guid TaskId, Guid AssigneeUserId)> List { get; set; }
}