using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Domain.Abstractions;

namespace EntryManager.Core.Accrual.Api.Domain.Interfaces;

public interface ILedgerRepository : IBaseRepository<LedgerModel, long>, ILedgerReadRepository
{
    
}