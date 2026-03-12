using System.Linq.Expressions;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class CategoryReadRepository : BaseReadRepository<CategoryModel, Guid>, ICategoryReadRepository
{
    private IGroupReadRepository _groupRepository;

    public CategoryReadRepository(IServiceProvider provider, AccrualContext context, IServiceBus serviceBus)
        : base(context, serviceBus, Collections.CATEGORY)
        => this._groupRepository = provider.GetRequiredService<IGroupReadRepository>();

    public override async Task<CategoryModel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await base.FindByIdAsync(id, cancellationToken);
        var group = await this._groupRepository.FindByIdAsync(category.GroupId, cancellationToken);
        
        return category is null ? null : new CategoryModel(category.Id, category.Name, group);
    }

    public override async Task<IEnumerable<CategoryModel>> FindAsync(Expression<Func<CategoryModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var categories = await base.FindAsync(predicate, cancellationToken);

        var groupIds = categories.Select(g => g.GroupId)
            .Distinct()
            .ToList();
        
        var groups = (await this._groupRepository.FindByIdAsync(groupIds, cancellationToken))
            .ToDictionary(a => a.Id);
        
        var finalCategories = categories
            .Select(category => groups.TryGetValue(category.GroupId, out var group) 
                    ? new CategoryModel(category.Id, category.Name, group) 
                    : category
            ).ToList();

        return finalCategories.ToList();
    }

    public override async Task<IEnumerable<CategoryModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await base.GetAllAsync(cancellationToken);

        var groupIds = categories.Select(g => g.GroupId)
            .Distinct();
        
        var accounts = (await this._groupRepository.FindByIdAsync(groupIds, cancellationToken))
            .ToDictionary(a => a.Id);
        
        var finalCategories = categories.Select(category => accounts.TryGetValue(category.GroupId, out var group) 
            ? new CategoryModel(category.Id, category.Name, group) 
            : category);

        return finalCategories.ToList();
    }
}