using analyticsService.Models;

namespace analyticsService.Repositories;

public class AnalyticsTransactionsRepository
{
    private List<AnalyticsTransaction> list = new();
    
    public static readonly AnalyticsTransactionsRepository Instance = new();

    private readonly Dictionary<Guid, List<AnalyticsTransaction>> dictionary = new();

    public void Add(AnalyticsTransaction transaction)
    {
        list.Add(transaction);  
    }
}