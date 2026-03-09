using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class CategoryRepository : BaseRepository<CategoryModel, Guid>, ICategoryRepository, ICategoryReadRepository
{
    public CategoryRepository(TransactionContext context, IServiceBus serviceBus, ICategoryReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.CATEGORY)
    {
    }
}