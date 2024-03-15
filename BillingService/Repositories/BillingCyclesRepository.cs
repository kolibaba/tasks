using tasksManager.Models;

namespace tasksManager.Repositories;

public class BillingCyclesRepository
{
    public static readonly BillingCyclesRepository Instance = new();
    
    private readonly List<BillingCycle> list = new();

    public BillingCycle CreateNext(BillingCycle billingCycleCurrent)
    {
        var billingCycle = new BillingCycle
        {
            Id = Guid.NewGuid(),
            FromDate = billingCycleCurrent.ToDate.AddSeconds(1),
            ToDate = billingCycleCurrent.ToDate.AddDays(1)
        };
        list.Add(billingCycle);
        return billingCycle;
    }
    
    public BillingCycle GetOrCreateCurrent()
    {
        var now = DateTime.Now;
        
        var found = Find(now);
        if (found != null)
        {
            return found;
        }

        var billingCycle = new BillingCycle
        {
            Id = Guid.NewGuid(),
            FromDate = now.Date,
            ToDate = now.Date.Add(TimeSpan.FromDays(1))
        };
        list.Add(billingCycle);
        
        return billingCycle;
    }
    
    public void SetManagerBalance(Guid billingCycleId, decimal balance)
    {
        var billingCycle = Get(billingCycleId);
        billingCycle.ManagerBalance = balance;
    }

    private BillingCycle Get(Guid billingCycleId)
    {
        return list.First(b => b.Id == billingCycleId);
    }

    BillingCycle? Find(DateTime dateTime)
    {  
        return list.FirstOrDefault(x => dateTime > x.FromDate && dateTime < x.ToDate);
    }
}