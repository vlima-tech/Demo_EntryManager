namespace EntryManager.Core.Accrual.Api.Domain.ValueObjects;

public record RollupGroup(Guid GroupId, string Name, long Balance)
{
    
}