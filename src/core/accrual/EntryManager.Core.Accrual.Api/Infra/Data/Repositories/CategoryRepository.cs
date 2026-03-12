using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class CategoryRepository : BaseRepository<CategoryModel, Guid>, ICategoryRepository, ICategoryReadRepository
{
    public CategoryRepository(AccrualContext context, IServiceBus serviceBus, ICategoryReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.CATEGORY)
    {
    }
}