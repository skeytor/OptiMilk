using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CattleManagement.API.HostedServices;

public sealed class CattleKafkaProducerService(
    IProducer<string, string> producer,
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<CattleKafkaProducerService> logger)
{
    private readonly KafkaSettings _kafkaSettings = kafkaSettings.Value;

    public async Task ProduceAsync<T>(string key, T value, CancellationToken ct = default)
        where T : notnull
    {
        string topic = _kafkaSettings.Topics.CattleEvents;
        try
        {
            string payload = JsonSerializer.Serialize(value);
            Message<string, string> msg = new() { Key = key, Value = payload };
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
