using Common.BrokerMessage;
using Common.Models;
using Common.Models.Events.Billing;
using tasksManager.Models;
using tasksManager.Repositories;

namespace tasksManager;

public class BillingService
{
    public static BillingService Instance = new BillingService();
    
    public async Task FinishBillingCycle()
    {
        var billingUsersRepository = BillingUsersRepository.Instance;
        var transactionsRepository = TransactionsRepository.Instance;
        var billingCyclesRepository = BillingCyclesRepository.Instance;
        
        var billingCycle = billingCyclesRepository.GetOrCreateCurrent();
        //var nextBillingCycle = billingCyclesRepository.CreateNext(billingCycle);

        //for users
        var transactionsDict = transactionsRepository.GetAllForCycle(billingCycle.Id);
        foreach (var kvp in transactionsDict)
        {
            var userId = kvp.Key;
            var transactions = kvp.Value;
            decimal userSumForDay = 0;
            transactions.ForEach(t =>
            {
                if (t.TransactionType == TransactionType.Debit)
                {
                    userSumForDay += t.Value;
                }
                else if (t.TransactionType == TransactionType.Credit)
                {
                    userSumForDay -= t.Value;
                }
            });
            var user = billingUsersRepository.GetUser(userId);
            var moneyToPayUser = user.Balance < 0 ? userSumForDay - user.Balance : userSumForDay; 
            if (moneyToPayUser > 0)
            {
                //log transaction to pay user
                var transaction = new Transaction
                {
                    TransactionType = TransactionType.Payment,
                    Value = moneyToPayUser,
                    Text = $"user got the money",
                    BillingCycleId = billingCycle.Id
                };
                transactionsRepository.Add(userId, transaction);
                await BrokerMessageProducer.Produce("billing", "billing/transaction_added", 
                    new TransactionAddedV1(new TransactionAddedV1Data
                    {
                        TransactionType = transaction.TransactionType,
                        Value = transaction.Value,
                        UserId = userId,
                        Text = transaction.Text
                    }));
                //TODO: + send email
                billingUsersRepository.SetBalance(userId, 0);
            }
        }
        
        //for management
        decimal managementBalance = 0;
        foreach (var kvp in transactionsDict)
        {
            var transactions = kvp.Value;
            transactions.ForEach(t =>
            {
                if (t.TransactionType == TransactionType.Credit)
                {
                    managementBalance += t.Value;
                }
                else if (t.TransactionType == TransactionType.Debit)
                {
                    managementBalance -= t.Value;
                }
            });
        }
    }
}