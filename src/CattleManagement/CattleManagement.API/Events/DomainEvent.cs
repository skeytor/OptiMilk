namespace CattleManagement.API.Events;

public abstract record DomainEvent(Guid StreamId)
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
};
