using Confluent.Kafka;

namespace CattleManagement.API.Services;

public class KafkaProducerService(IProducer<string, string> producer)
{
    private readonly IProducer<string, string> _producer = producer;

    public async Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken = default)
    {
        Message<string, string> msg = new() { Key = key, Value = value };
        await _producer.ProduceAsync(topic, msg, cancellationToken);
        _producer.Flush(TimeSpan.FromSeconds(5));
    }
}
