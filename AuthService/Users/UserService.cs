using Common.BrokerMessage;
using Common.Models;
using Common.Models.Events.Accouns;
using Common.Models.Events.Accouns.Streaming;

namespace AuthService.Users;

public class UserService
{
    public static readonly UserService Instance = new();

    public async Task Init()
    {
        var usersRepository = UsersRepository.Instance;
        
        foreach (var user in usersRepository.GetAll())
            await BrokerMessageProducer.Produce(
                AccountStreamingConstants.TopicName,
                "accounts/streaming_account_created",
                new AccountStreamingCreatedV1(
                    new AccountStreamingCreatedDataV1
                    {
                        UserId = user.Id,
                        Role = user.Role,
                        Email = user.Email
                    }));
    }

    public async Task<User> Create(string login, string password, string role)
    {
        var usersRepository = UsersRepository.Instance; //TODO: use real Db later and DI

        var user = new User(Guid.NewGuid(), login, password, new Role(role));
        usersRepository.Add(user);

        await BrokerMessageProducer.Produce(
            AccountStreamingConstants.TopicName,
            "accounts/streaming_account_created",
            new AccountStreamingCreatedV1(
                new AccountStreamingCreatedDataV1
                {
                    UserId = user.Id,
                    Role = user.Role,
                    Email = user.Email
                }));

        await BrokerMessageProducer.Produce(
            AccountStreamingConstants.TopicName,
            "accounts/streaming_account_created",
            new AccountStreamingCreatedV1(
                new AccountStreamingCreatedDataV1
                {
                    UserId = user.Id,
                    Role = user.Role,
                    Email = user.Email
                }));

        await BrokerMessageProducer.Produce(
            AccountEventsConstants.TopicName,
            "accounts/account_registered",
            new AccountRegisteredV1(
                new AccountRegisteredDataV1
                {
                    UserId = user.Id,
                    Role = user.Role,
                    Email = user.Email
                }));

        return user;
    }
}