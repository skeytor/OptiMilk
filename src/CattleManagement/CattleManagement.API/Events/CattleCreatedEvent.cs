namespace CattleManagement.API.Events;

public sealed record CattleCreatedEvent(Guid CattleId, string TagNumber, DateOnly DateOfBirth) 
    : DomainEvent(CattleId);
