using System.Net;
using System.Text.Json;
using Common.Models.Events;
using Confluent.Kafka;
using EventSchemaRegistry;

namespace Common.BrokerMessage;

public class BrokerMessageProducer
{
    private static readonly ProducerConfig config = new()
    {
        BootstrapServers = "localhost:9092",
        ClientId = Dns.GetHostName()
    };

    public static async Task Produce<TEvent>(string topic, string schemaPath, TEvent eventItem) where TEvent : IEventBase
    {
        using var producer = new ProducerBuilder<string, string>(config).Build();

        //проверяем схему прямо producer-e. Вся инфа (кроме пути к схеме) есть в ДТО-шке события.
        SchemaRegistry.ValidateEvent(eventItem, schemaPath, eventItem.Version);

        var result = await producer.ProduceAsync(
            topic,
            new Message<string, string>
            {
                Key = eventItem.Name,
                Value = JsonSerializer.Serialize(eventItem)
            });
    }
}