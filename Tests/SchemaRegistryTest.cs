using Common.Models.Events;
using Common.Models.Events.Accouns;
using Common.Models.Events.Accouns.Streaming;
using Common.Models.Events.Tasks;
using Common.Models.Events.Tasks.Streaming;
using EventSchemaRegistry;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ValidateEvent()
    {
        var result = SchemaRegistry.ValidateEvent(new AccountStreamingCreatedV1(new AccountStreamingCreatedDataV1()),
            "accounts/streaming_account_created", 1);
        var result2 = SchemaRegistry.ValidateEvent(new AccountRegisteredV1(new AccountRegisteredDataV1()),
            "accounts/account_registered", 1);

        SchemaRegistry.ValidateEvent(new TaskStreamingCreatedV1(new TaskStreamCreatedDataV1()),
            "tasks/streaming_task_created", 1);
        SchemaRegistry.ValidateEvent(new TaskCreatedV1(new TaskCreatedDataV1()), "tasks/task_created", 1);
        SchemaRegistry.ValidateEvent(new TaskCompletedV1(new TaskCompletedDataV1()), "tasks/task_completed", 1);
        SchemaRegistry.ValidateEvent(new TasksShuffledV1(new TasksShuffledDataV1()), "tasks/task_shuffled", 1);
        Assert.IsTrue(result);
    }
}