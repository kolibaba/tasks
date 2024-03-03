namespace tasksManager.Users;

public class UsersRepository
{
    private readonly List<User> _users = new();

    //TODO: use real Db later
    private UsersRepository()
    {
        _users = new List<User>
        {
            new("tom@gmail.com", "12345", new Role("admin")),
            new("bob@gmail.com", "55555", new Role("user")),
            new("mary@gmail.com", "55555", new Role("user"))
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