namespace Common.Models;

public class UsersStream
{
    public const string TopicName = "users-stream";

    public class UserCreated
    {
        public const string EventName = "UserCreated";
        public Guid UserId { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }
    }
}