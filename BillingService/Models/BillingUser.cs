namespace tasksManager.Models;

public class BillingUser
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
    public decimal Balance { get; set; }
}