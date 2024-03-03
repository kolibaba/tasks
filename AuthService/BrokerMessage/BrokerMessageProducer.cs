using System.Net;
using Confluent.Kafka;

public class BrokerMessageProducer
{
    public static async Task Produce(string topic, string value)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            ClientId = Dns.GetHostName()
        };
        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var result = await producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = value
        });
    }
}