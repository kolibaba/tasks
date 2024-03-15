using tasksManager.Models;

namespace tasksManager.Repositories;

public class BillingUsersRepository
{
    public static readonly BillingUsersRepository Instance = new();

    private readonly List<BillingUser> list = new();

    public void Add(BillingUser billingUser)
    {
        list.Add(billingUser);
    }

    public BillingUser GetUser(Guid assigneeUserId)
    {
        return list.First(x => x.UserId == assigneeUserId);
    }

    public void SetBalance(Guid userId, decimal balance)
    {
        GetUser(userId).Balance = balance;
    }
}