namespace MilkingYield.API.Events;

public sealed record CattleDeletedEvent(
    Guid CattleId,
    string EventType);