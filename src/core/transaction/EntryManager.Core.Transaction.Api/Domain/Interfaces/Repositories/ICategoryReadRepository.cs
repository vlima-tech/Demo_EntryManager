using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Shared.Domain.Abstractions;

namespace EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;

public interface ICategoryReadRepository : IBaseReadRepository<CategoryModel, Guid>
{
    
}