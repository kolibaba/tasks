using tasksManager.Models;

namespace tasksManager.Repositories;

public class TransactionsRepository
{
    public static readonly TransactionsRepository Instance = new();

    private readonly Dictionary<Guid, List<Transaction>> dictionary = new();

    public void Add(Guid userId, TransactionType transactionType, decimal value, string text)
    {
        GetListFor(userId).Add(new Transaction
        {
            TransactionType = transactionType,
            Value = value,
            Text = text
        });
    }

    public List<Transaction> GetAll(Guid userId)
    {
        return GetListFor(userId);
    }

    private List<Transaction> GetListFor(Guid userId)
    {
        if (dictionary.TryGetValue(userId, out var list)) return list;

        list = new List<Transaction>();
        dictionary.Add(userId, list);
        return list;
    }
}