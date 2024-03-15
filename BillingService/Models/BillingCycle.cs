namespace tasksManager.Models;

public class BillingCycle
{
    public Guid Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal ManagerBalance { get; set; }
}