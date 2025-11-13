using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CattleManagement.API.Services;

public class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IConsumer<string, string> _consumer;
    public KafkaConsumerBackgroundService(
        ILogger<KafkaConsumerBackgroundService> logger,
        IOptions<KafkaSettings> options)
    {
        _logger = logger;
        _kafkaSettings = options.Value;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
            .Build();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string topic = _kafkaSettings.Topics.MilkingEvents;
        _consumer.Subscribe(topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(stoppingToken);
                    _logger.LogInformation("Received message: {Key} => {Value} at {Offset}", cr.Message.Key, cr.Message.Value, cr.TopicPartitionOffset);
                    _consumer.Commit(cr);
                }
                catch (ConsumeException ce)
                {
                    _logger.LogError(ce, "Consume error");
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _consumer.Close();
        }
    }
}