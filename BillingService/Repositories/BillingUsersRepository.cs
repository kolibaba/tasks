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
}