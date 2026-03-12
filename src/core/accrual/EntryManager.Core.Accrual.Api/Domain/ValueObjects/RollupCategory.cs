namespace EntryManager.Core.Accrual.Api.Domain.ValueObjects;

public record RollupCategory(Guid CategoryId, string Title, long Balance)
{
    
}