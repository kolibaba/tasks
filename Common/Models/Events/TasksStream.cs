namespace Common.Models;

public class TasksStream
{
    public const string TopicName = "tasks-stream";

    public class TaskCreated
    {
        public const string EventName = "TaskCreated";
        public Guid TaskId { get; set; }
        public string TaskText { get; set; }
    }
}