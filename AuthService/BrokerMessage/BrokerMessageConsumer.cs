using System.Text.Json;
using Confluent.Kafka;

public class BrokerMessageConsumer
{
    private static readonly ConsumerConfig config = new()
    {
        BootstrapServers = "localhost:9092",
        GroupId = "test-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    public static void Consume(string topic, Action<string> handle)
    {
        try
        {
            using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
            consumerBuilder.Subscribe(topic);
            var cancelToken = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    var consumer = consumerBuilder.Consume(cancelToken.Token);
                    handle(consumer.Message.Value);
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