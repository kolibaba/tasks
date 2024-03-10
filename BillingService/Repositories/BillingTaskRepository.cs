using tasksManager.Models;

namespace tasksManager.Repositories;

public class BillingTaskRepository
{
    public static readonly BillingTaskRepository Instance = new();

    private readonly List<BillingTask> list = new();

    public void Add(BillingTask billingTask)
    {
        list.Add(billingTask);
    }

    public BillingTask? Find(Guid tasId)
    {
        return list.FirstOrDefault(x => x.TaskId == tasId);
    }
}