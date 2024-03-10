namespace tasksManager.Models;

public class Transaction
{
    public TransactionType TransactionType { get; set; }
    public decimal Value { get; set; }

    public string Text { get; set; }
    //TODO add BillingCycleId ?
}