namespace CattleManagement.API.Events;

public sealed record CattleDeletedEvent(Guid CattleId, string TagNumber) : DomainEvent(CattleId);
