using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MilkingYield.API.Handlers;

namespace MilkingYield.API.HostedServices;

public class MilkingYieldConsumerBackgroundService(
    ILogger<MilkingYieldConsumerBackgroundService> logger,
    IConsumer<string, string> consumer,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaSettings> options)
    : BackgroundService
{
    private readonly KafkaSettings _kafkaSettings = options.Value;
    private Dictionary<string, IKafkaEventHandler> _eventHandlerMap = null!;    

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
                    var cr = await Task.Run(() => consumer.Consume(stoppingToken), stoppingToken);
                    //var cr = consumer.Consume(stoppingToken);
                    if (cr?.Message?.Value is null)
                    {
                        continue;
                    }
                    await ProcessMessageAsync(cr, stoppingToken);
                }
                catch (ConsumeException ce)
                {
                    logger.LogError(ce, "Consume error");
                }
            }
        }
        catch (OperationCanceledException) 
        { 
            logger.LogInformation("Kafka consumer cancellation requested");
        }
        finally
        {
            consumer.Close();
            logger.LogInformation("Kafka consumer stopped");
        }
    }
    private async Task ProcessMessageAsync(ConsumeResult<string, string> cr, CancellationToken ct)
    {
        bool shouldCommit = false;
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IEnumerable<IKafkaEventHandler> handlers = scope.ServiceProvider
            .GetServices<IKafkaEventHandler>();

        _eventHandlerMap = handlers.ToDictionary(h => h.EventType, h => h);

        try
        {
            string message = cr.Message.Value;
            JsonDocument jsonDoc = JsonDocument.Parse(message);
            if (jsonDoc.RootElement.TryGetProperty("EventType", out var eventTypeElement))
            {
                string? eventType = eventTypeElement.GetString();
                if (eventType is not null && _eventHandlerMap.TryGetValue(eventType, out var handler))
                {
                    await handler.HandleAsync(message, ct);
                    shouldCommit = true;
                    logger.LogInformation("Processed message with EventType: {EventType}", eventType);
                }
                else
                {
                    logger.LogWarning("No handler found for EventType: {EventType}", eventType);
                    shouldCommit = true; // Commit to skip unhandled event types
                }
            }
            else
            {
                logger.LogWarning("EventType property not found in message");
                shouldCommit = true; // Commit to skip malformed messages
            }
        }
        catch (JsonException je)
        {
            logger.LogError(je, "JSON parsing error");
            shouldCommit = true; // Commit to skip malformed messages
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message");
        }

        if (shouldCommit)
        {
            consumer.Commit(cr);
            logger.LogInformation("Committed offset for message at {TopicPartitionOffset}", cr.TopicPartitionOffset);
        }
    }
}