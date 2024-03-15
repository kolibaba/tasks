using Common.BrokerMessage;
using Common.Models.Events;
using Common.Models.Events.Accouns.Streaming;
using Common.Models.Events.Tasks;
using Common.Models.Events.Tasks.Streaming;

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
        //listen streaming event from Accounting
        BrokerMessageConsumer.Consume<AccountStreamingCreatedV1>(
            AccountStreamingConstants.TopicName,
            AccountStreamingConstants.EventNames.AccountCreated,
            "accounts/account_streaming_created",
            1,
            eventDto =>
            {
                _users.Add(new TaskUser
                {
                    Id = eventDto.Data.UserId,
                    Name = eventDto.Data.Email
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
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
        _tasks.Add(taskItem);

        //Streaming-Event создания таски
        await BrokerMessageProducer.Produce(
            TasksStreamConstants.TopicName,
            "tasks/streaming_task_created",
            new TaskStreamingCreatedV1(
                new TaskStreamCreatedDataV1
                {
                    TaskId = taskItem.Id,
                    TaskText = taskItem.Text,
                    CreatedAt = taskItem.CreatedAt
                }));

        //BE создания таски
        await BrokerMessageProducer.Produce(TasksEventsConstants.TopicName, "tasks/task_created",
            new TaskCreatedV1(
                new TaskCreatedDataV1
                {
                    TaskId = taskItem.Id,
                    AssigneeUserId = userAssignee.Id
                }));

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

        var data = new TaskCompletedDataV1
        {
            TaskId = taskItem.Id,
            ByUserId = byUserId
        };
        await BrokerMessageProducer.Produce(TasksEventsConstants.TopicName, "tasks/task_completed",
            new TaskCompletedV1(data));

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

        var data = new TasksShuffledDataV1
        {
            List = list
        };
        await BrokerMessageProducer.Produce(TasksEventsConstants.TopicName, "tasks/tasks_shuffled",
            new TasksShuffledV1(data));
    }
}