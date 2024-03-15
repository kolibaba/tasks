namespace tasksManager.Tasks;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public TaskUser UserAssignee { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}