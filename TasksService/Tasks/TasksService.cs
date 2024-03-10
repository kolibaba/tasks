using Common.BrokerMessage;
using Common.Models;

namespace tasksManager.Tasks;

public class TasksService
{
    private static readonly List<TaskUser> _users = new();

    public static readonly TasksService Instance = new();
    private readonly List<TaskItem> _tasks = new();
    private readonly Random random = new();

    private TasksService()
    {
    }

    public List<TaskItem> GetAllTasks()
    {
        return _tasks;
    }

    public void Init()
    {
        //default for init
        var id = Guid.Parse("5CCCA3A2-EC58-4BD8-9718-8B04D77467AC");
        _users.Add(new TaskUser
        {
            Id = id,
            Name = "bob@gmail.com"
        });
        Task.Run(ConsumeEvents);
    }

    private static void ConsumeEvents()
    {
        //listen cud event from Auth
        BrokerMessageConsumer.Consume<UsersStream.UserCreated>(UsersStream.TopicName, UsersStream.UserCreated.EventName,
            eventDto =>
            {
                _users.Add(new TaskUser
                {
                    Id = eventDto.UserId,
                    Name = eventDto.Email
                });
            });
    }

    public async Task<TaskItem> Create(string text)
    {
        //TODO: как тут ждать инициализации, когда юзеров еще нет и добаили первых?
        var userAssignee = FindRandomUser();

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Text = text,
            UserAssignee = userAssignee,
            IsCompleted = false
        };
        _tasks.Add(taskItem);

        await BrokerMessageProducer.Produce(TasksStream.TopicName, TasksStream.TaskCreated.EventName,
            new TasksStream.TaskCreated
            {
                TaskId = taskItem.Id,
                TaskText = taskItem.Text
            });

        await BrokerMessageProducer.Produce(TasksEvents.TopicName, TasksEvents.TaskAssigned.EventName,
            new TasksEvents.TaskAssigned
            {
                TaskId = taskItem.Id,
                AssigneeUserId = userAssignee.Id
            });

        return taskItem;
    }

    private TaskUser FindRandomUser()
    {
        if (_users.Count == 0)
            throw new InvalidOperationException("users is empty. Couldn't find user for task");
        var index = random.Next(0, _users.Count);
        var userAssignee = _users[index];
        return userAssignee;
    }

    public async Task Complete(Guid taskId, Guid byUserId)
    {
        var taskItem = _tasks.Find(t => t.Id == taskId);
        if (taskItem == null) throw new ArgumentException($"Task with id={taskId} is not found");

        await BrokerMessageProducer.Produce(TasksEvents.TopicName, TasksEvents.TaskAssigned.EventName,
            new TasksEvents.TaskCompleted
            {
                TaskId = taskItem.Id,
                ByUserId = byUserId
            });

        taskItem.IsCompleted = true;
    }

    public async Task Shuffle()
    {
        var list = new List<(Guid TaskId, Guid AssigneeUserId)>();
        foreach (var taskItem in _tasks.Where(t => !t.IsCompleted))
        {
            var user = FindRandomUser();
            list.Add((taskItem.Id, user.Id));
        }

        await BrokerMessageProducer.Produce(TasksEvents.TopicName, TasksEvents.TasksShuffled.EventName,
            new TasksEvents.TasksShuffled
            {
                List = list
            });
    }
}