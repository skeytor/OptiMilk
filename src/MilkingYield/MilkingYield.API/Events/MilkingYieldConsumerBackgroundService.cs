using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MilkingYield.API.Services;

namespace MilkingYield.API.Events;

public class MilkingYieldConsumerBackgroundService(
    ILogger<MilkingYieldConsumerBackgroundService> logger,
    MilkingSessionService milkingSessionService,
    IConsumer<string, string> consumer,
    IOptions<KafkaSettings> options)
    : BackgroundService
{
    private readonly KafkaSettings _kafkaSettings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string topic = _kafkaSettings.Topics.CattleEvents;
        consumer.Subscribe(topic);
        logger.LogInformation("Kafka consumer started for topic: {topic}", topic);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    if (cr?.Message.Value is null)
                    {
                        continue;
                    }
                    CattleDeletedEvent? evt = null;
                    try
                    {
                        evt = JsonSerializer.Deserialize<CattleDeletedEvent>(cr.Message.Value);
                    }
                    catch (JsonException jex)
                    {
                        logger.LogError(jex, "Failed to deserialize message value");
                    }
                    
                    if (evt != null)
                    {
                        await milkingSessionService.DeleteByCowIdAsync(evt.CattleId);
                        consumer.Commit(cr); // commit after successful processing
                    }
                    else
                    {
                        // Optionally commit or move to dead-letter handling
                        consumer.Commit(cr);
                    }
                    logger.LogInformation("Received message: {Key} => {Value} at {Offset}", cr.Message.Key, cr.Message.Value, cr.TopicPartitionOffset);
                    consumer.Commit(cr);
                }
                catch (ConsumeException ce)
                {
                    logger.LogError(ce, "Consume error");
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            consumer.Close();
            logger.LogInformation("Kafka consumer stopped");
        }
    }
}