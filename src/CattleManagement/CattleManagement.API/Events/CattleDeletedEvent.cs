namespace CattleManagement.API.Events;

public sealed record CattleDeletedEvent(Guid CattleId, string TagNumber) : DomainEvent(CattleId);
public sealed record CattleCreatedEvent(Guid CattleId, string TagNumber, DateOnly DateOfBirth) 
    : DomainEvent(CattleId);
public sealed record CattleUpdatedEvent(Guid CattleId, string TagNumber, DateOnly DateOfBirth) 
    : DomainEvent(CattleId);
