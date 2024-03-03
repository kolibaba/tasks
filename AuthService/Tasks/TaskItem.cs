namespace tasksManager.Tasks;

public class TaskItem
{
    public string Text { get; set; }
    public TaskUser UserAssignee { get; set; }
    public bool IsCompleted { get; set; }
}