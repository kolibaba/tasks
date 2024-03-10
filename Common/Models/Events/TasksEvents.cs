namespace Common.Models;

public class TasksEvents
{
    public const string TopicName = "tasks";

    public class TaskAssigned
    {
        public const string EventName = "TaskAssigned";
        public Guid TaskId { get; set; }
        public Guid AssigneeUserId { get; set; }
    }

    public class TaskCompleted
    {
        public const string EventName = "TaskCompleted";
        public Guid TaskId { get; set; }
        public Guid ByUserId { get; set; }
    }

    public class TasksShuffled
    {
        public const string EventName = "TasksShuffled";
        public List<(Guid TaskId, Guid AssigneeUserId)> List { get; set; }
    }
}