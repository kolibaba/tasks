namespace tasksManager.Models;

public class BillingTask
{
    public Guid TaskId { get; init; }
    public string TaskText { get; set; }
    public decimal AssignPrice { get; init; }
    public decimal CompletePrice { get; init; }
}