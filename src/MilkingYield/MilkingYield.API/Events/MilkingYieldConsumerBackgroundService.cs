using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MilkingYield.API.Handlers;
using MilkingYield.API.Services;

namespace MilkingYield.API.Events;

public class MilkingYieldConsumerBackgroundService(
    ILogger<MilkingYieldConsumerBackgroundService> logger,
    IConsumer<string, string> consumer,
    IEnumerable<IKafkaEventHandler> eventHandlers,
    IOptions<KafkaSettings> options)
    : BackgroundService
{
    private readonly KafkaSettings _kafkaSettings = options.Value;
    private readonly Dictionary<string, IKafkaEventHandler> _eventHandlerMap =
        eventHandlers.ToDictionary(h => h.EventType, h => h);

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
                    ConsumeResult<string, string> cr = consumer.Consume(stoppingToken);
                    if (cr?.Message.Value is null)
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
                    logger.LogInformation("Processed message with EventType: {EventType}", eventType);
                }
                else
                {
                    logger.LogWarning("No handler found for EventType: {EventType}", eventType);
                }
            }
            else
            {
                logger.LogWarning("EventType property not found in message");
            }
        }
        catch (JsonException je)
        {
            logger.LogError(je, "JSON parsing error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message");
        }
    }
}