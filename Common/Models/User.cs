namespace Common.Models;

public class User
{
    public User(Guid id, string email, string password, Role role)
    {
        Id = id;
        Email = email;
        Password = password;
        Role = role;
    }

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
}