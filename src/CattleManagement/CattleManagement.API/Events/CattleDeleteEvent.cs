namespace CattleManagement.API.Events;

public sealed record CattleDeleteEvent(
    Guid CattleId,
    DateTime DeletedAt
);
