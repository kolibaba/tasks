internal class Person
{
    public Person(string email, string password, Role role)
    {
        Email = email;
        Password = password;
        Role = role;
    }

    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
}