namespace Common.Models.Events.Billing;

public class TransactionAddedV1 : EventBase<TransactionAddedV1Data>
{
    public TransactionAddedV1(TransactionAddedV1Data data) : base(EventsNames.TransactionAdded, 1, data)
    {
    }
}

public record TransactionAddedV1Data
{
    public Guid UserId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Value { get; set; }
    public string Text { get; set; }
}