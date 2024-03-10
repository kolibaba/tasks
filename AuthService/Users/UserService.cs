using Common.Models;

namespace AuthService.Users;

public class UserService
{
    public static readonly UserService Instance = new();

    public async Task Init()
    {
        var usersRepository = UsersRepository.Instance;
        foreach (var user in usersRepository.GetAll())
            await BrokerMessageProducer.Produce(UsersStream.TopicName, UsersStream.UserCreated.EventName,
                new UsersStream.UserCreated
                {
                    UserId = user.Id,
                    Role = user.Role,
                    Email = user.Email
                });
    }

    public async Task<User> Create(string login, string password, string role)
    {
        var usersRepository = UsersRepository.Instance; //TODO: use real Db later and DI

        var user = new User(Guid.NewGuid(), login, password, new Role(role));
        usersRepository.Add(user);

        await BrokerMessageProducer.Produce(UsersStream.TopicName, UsersStream.UserCreated.EventName,
            new UsersStream.UserCreated
            {
                UserId = user.Id,
                Role = user.Role,
                Email = user.Email
            });

        return user;
    }
}