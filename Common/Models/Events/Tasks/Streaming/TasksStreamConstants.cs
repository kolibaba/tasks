namespace Common.Models.Events.Tasks.Streaming;

public class TasksStreamConstants
{
    public const string TopicName = "tasks-stream";

    public class EventNames
    {
        public const string TaskCreated = "TaskCreated";
        public const string TaskPriceUpdated = "TaskPriceUpdated";
    }
}