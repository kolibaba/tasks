using Common.Models;

namespace AuthService.Users;

public class UsersRepository
{
    private readonly List<User> _users = new();

    //TODO: use real Db later
    private UsersRepository()
    {
        _users = new List<User>
        {
            new(Guid.Parse("36CC6F48-0D34-4920-BE07-D1E29DEEF2D7"), "tom@gmail.com", "12345", new Role("admin")),
            new(Guid.Parse("5CCCA3A2-EC58-4BD8-9718-8B04D77467AC"), "bob@gmail.com", "12345", new Role("user")),
            new(Guid.Parse("BAAC9BB7-A87B-49F8-B8C1-E02C90B2BE28"), "mary@gmail.com", "12345", new Role("user"))
        };
    }

    public static UsersRepository Instance => new();

    public List<User> GetAll()
    {
        return _users;
    }

    public void Add(User user)
    {
        _users.Add(user);
    }
}