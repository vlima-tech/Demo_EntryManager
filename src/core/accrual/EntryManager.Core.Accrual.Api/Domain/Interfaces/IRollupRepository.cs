using EntryManager.Core.Accrual.Api.Domain.ValueObjects;

namespace EntryManager.Core.Accrual.Api.Domain.Interfaces;

public interface IRollupRepository
{
    bool Exists(DateTime rollupDay);
    
    Task<bool> InitializeAsync(Rollup rollup, CancellationToken cancellationToken = default);
    
    Task<Rollup> LoadAsync(DateTime rollupDay, CancellationToken cancellationToken = default);
    
    Task<bool> RemoveAsync(DateTime rollupDay, CancellationToken cancellationToken = default);
}