namespace MilkingYield.API.Events;

public sealed record CattleDeletedEvent(
    Guid CattleId,
    string TagNumber,
    string EventType);