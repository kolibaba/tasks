namespace Common.Models.Events.Accouns.Streaming;

public class AccountStreamingCreatedV1 : EventBase<AccountStreamingCreatedDataV1>
{
    public AccountStreamingCreatedV1(AccountStreamingCreatedDataV1 data)
        : base(AccountStreamingConstants.EventNames.AccountCreated, 1, data)
    {
    }
}

public class AccountStreamingCreatedDataV1
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
    public string Email { get; set; }
}