using System.Net;
using Confluent.Kafka;

namespace tasksManager.Users;

public class UserService
{
    public static UserService Instance => new();

    public async Task Init()
    {
        var usersRepository = UsersRepository.Instance;
        foreach (var user in usersRepository.GetAll())
        {
            await BrokerMessageProducer.Produce(BrokerMessageTopics.CreateUser, user.Email);
        }
    }
    
    public async Task<User> Create(string login, string password, string role)
    {
        var usersRepository = UsersRepository.Instance; //TODO: use real Db later and DI
        var user = new User(login, password, new Role(role));
        usersRepository.Add(user);

        await BrokerMessageProducer.Produce(BrokerMessageTopics.CreateUser, user.Email);

        return user;
    }
}