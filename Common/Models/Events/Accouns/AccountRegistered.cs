namespace Common.Models.Events.Accouns;

public class AccountRegisteredV1 : EventBase<AccountRegisteredDataV1>
{
    public AccountRegisteredV1(AccountRegisteredDataV1 data)
        : base(AccountEventsConstants.EventNames.AccountRegistered, 1, data)
    {
    }
}

public class AccountRegisteredDataV1
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
    public string Email { get; set; }
}