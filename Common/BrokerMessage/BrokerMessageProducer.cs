using System.Net;
using System.Text.Json;
using Confluent.Kafka;

public class BrokerMessageProducer
{
    private static readonly ProducerConfig config = new()
    {
        BootstrapServers = "localhost:9092",
        ClientId = Dns.GetHostName()
    };

    public static async Task Produce<T>(string topic, string @event, T value)
    {
        using var producer = new ProducerBuilder<string, string>(config).Build();

        var result = await producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = @event,
            Value = JsonSerializer.Serialize(value)
        });
    }
}