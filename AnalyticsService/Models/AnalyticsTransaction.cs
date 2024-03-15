using Common.Models;

namespace analyticsService.Models;

public class AnalyticsTransaction
{
    public TransactionType TransactionType { get; set; }
    public decimal Value { get; set; }
    public string Text { get; set; }
    public Guid UserId { get; set; }
}