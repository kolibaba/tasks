namespace analyticsService.Models;

public class AnalyticsTask
{
    public Guid TaskId { get; init; }
    public string TaskText { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal AssignPrice { get; set; }
    public decimal CompletePrice { get; set; }
}