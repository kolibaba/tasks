using System.Text.Json;
using Confluent.Kafka;

namespace Common.BrokerMessage;

public class BrokerMessageConsumer
{
    private static readonly ConsumerConfig config = new()
    {
        BootstrapServers = "localhost:9092",
        GroupId = "test-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    public static void Consume<T>(string topic, string @event, Action<T> handle)
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
                    if (consumer.Message.Key == @event)
                    {
                        var eventObj = JsonSerializer.Deserialize<T>(consumer.Message.Value);
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