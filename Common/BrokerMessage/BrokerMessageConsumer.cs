using System.Text.Json;
using Common.Models.Events;
using Confluent.Kafka;
using EventSchemaRegistry;

namespace Common.BrokerMessage;

public class BrokerMessageConsumer
{
    private static readonly ConsumerConfig config = new()
    {
        BootstrapServers = "localhost:9092",
        GroupId = "test-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    public static void Consume<TEvent>(string topic, string eventName, string schemaPath, int version,
        Action<TEvent> handle) where TEvent : IEventBase
    {
        ConsumeAsync<TEvent>(topic, eventName, schemaPath, version, ev =>
        {
            handle(ev);
            return Task.CompletedTask;
        });
    }
    
    public static void ConsumeAsync<TEvent>(string topic, string eventName, string schemaPath, int version,
        Func<TEvent, Task> handle) where TEvent : IEventBase
    {
        try
        {
            using var consumerBuilder = new ConsumerBuilder<string, string>(config).Build();
            consumerBuilder.Subscribe(topic);
            var cancelToken = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    var consumer = consumerBuilder.Consume(cancelToken.Token);
                    if (consumer.Message.Key == eventName)
                    {
                        var messageValue = consumer.Message.Value;
                        var eventObj = JsonSerializer.Deserialize<TEvent>(messageValue);
                        if (eventObj!.Version != version)
                            continue;
                        //то, что пришло в событии проверяем по схеме
                        SchemaRegistry.ValidateEvent(eventObj, schemaPath, version);
                        if (eventObj != null) handle(eventObj);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumerBuilder.Close();
            }
        }
        catch (Exception ex)
        {
            // Console.WriteLine(ex.Message);
        }
    }
}