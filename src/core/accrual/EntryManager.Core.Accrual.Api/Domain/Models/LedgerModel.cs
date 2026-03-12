using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Accrual.Api.Domain.Models;

public class LedgerModel : BaseModel<long>
{
    public Guid ReferenceId { get; private set; }

    public int EffectiveDay { get; private set; }
    
    public ReferenceType Type { get; private set; }

    public long Value { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }
    
    protected LedgerModel() { }
    
    public LedgerModel(Guid referenceId, DateTime effectedAt, ReferenceType referenceType, int value)
    {
        this.Id = GenerateId(referenceId, effectedAt);
        this.ReferenceId = referenceId;
        this.EffectiveDay = int.Parse(effectedAt.ToString("yyyyMMdd"));
        this.Type = referenceType;
        this.Value = value;
        this.CreatedAt = DateTime.UtcNow;
    }
    
    public static long GenerateId(Guid referenceId, DateTime referenceDate)
    {
        var ledgerHash = int.Parse(referenceDate.ToString("yyyyMMdd"));
        var guidHash = (uint)referenceId.GetHashCode();

        return (ledgerHash * 10_000_000_000L) + guidHash;
    }
}