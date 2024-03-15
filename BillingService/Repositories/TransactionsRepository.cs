using Common.Models;
using tasksManager.Models;

namespace tasksManager.Repositories;

public class TransactionsRepository
{
    public static readonly TransactionsRepository Instance = new();

    private readonly Dictionary<Guid, List<Transaction>> dictionary = new();

    public void Add(Guid userId, Transaction transaction)
    {
        GetListFor(userId).Add(transaction);  
    }

    public Dictionary<Guid, List<Transaction>> GetAllForCycle(Guid billingCycleId)
    {
        Dictionary<Guid, List<Transaction>> resultDict = new();
        foreach (var kvp in dictionary)
        {
            var userId = kvp.Key;
            var transactions = kvp.Value;
            var list = transactions.Where(t => t.BillingCycleId == billingCycleId).ToList();
            if (list.Count > 0)
            {
                resultDict[userId] = list;
            }
        }
        return resultDict;
    }

    private List<Transaction> GetListFor(Guid userId)
    {
        if (dictionary.TryGetValue(userId, out var list)) return list;

        list = new List<Transaction>();
        dictionary.Add(userId, list);
        return list;
    }
}