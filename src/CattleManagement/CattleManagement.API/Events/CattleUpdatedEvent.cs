namespace CattleManagement.API.Events;

public sealed record CattleUpdatedEvent(Guid CattleId, string TagNumber, DateOnly DateOfBirth) 
    : DomainEvent(CattleId);
