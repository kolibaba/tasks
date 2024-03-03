using System.Text.Json;

namespace tasksManager.Tasks;

public class TasksService
{
    private readonly List<TaskItem> _tasks = new();
    private static readonly List<TaskUser> _users = new();

    private readonly Random random = new();
    public static TasksService Instance => new();

    private TasksService()
    {
    }

    public async void Init()
    {
        //File.AppendAllText(@"c:\temp\log_kafka.txt", "enter TasksService.SubscribeOnEvents" + "\n");
        await Task.Run(() =>
        {
            BrokerMessageConsumer.Consume(BrokerMessageTopics.CreateUser, login =>
            {
               // File.AppendAllText(@"c:\temp\log_kafka.txt", login + "\n");
                _users.Add(new TaskUser
                {
                    Name = login
                });
            });
        });
    }

    public void Create(string text)
    {
        //TODO: как тут ждать инициализации, когда юзеров еще нет и добаили первых?
        if (_users.Count == 0)
            throw new InvalidOperationException("users is empty. Couldn't find user for task");
        var index = random.Next(0, _users.Count);
        var randomUser = _users[index];

        _tasks.Add(new TaskItem
        {
            Text = text,
            UserAssignee = randomUser,
            IsCompleted = false
        });
        //TODO: add message to broker
    }
}