namespace MilkingYield.API.Handlers;

public interface IKafkaEventHandler
{
    string EventType { get; }
    Task HandleAsync(string message, CancellationToken ct = default);
}
