using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CattleManagement.API.Events;

public sealed class CattleKafkaProducerService(
    IProducer<string, string> producer,
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<CattleKafkaProducerService> logger)
{
    private readonly KafkaSettings _kafkaSettings = kafkaSettings.Value;

    public async Task ProduceAsync(string key, object value, CancellationToken ct = default)
    {
        string topic = _kafkaSettings.Topics.CattleEvents;
        try
        {
            string jsonValue = JsonSerializer.Serialize(value);
            Message<string, string> msg = new() { Key = key, Value = jsonValue };
            DeliveryResult<string, string> result = await producer.ProduceAsync(topic, msg, ct);
            logger.LogInformation("Produced message to topic {Topic}: {Key} - {Value}", topic, key, value);
        }
        catch (ProduceException<string, string> ex)
        {
            logger.LogError("Failed to deliver message: {Error}", ex.Error.Reason);
            throw;
        }
    }
}
