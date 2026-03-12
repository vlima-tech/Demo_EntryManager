using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Core.Transaction.Contracts.Responses.CategoryResponses;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class CategoryReadRepository : BaseReadRepository<CategoryModel, Guid>, ICategoryReadRepository, ICategoryQuery
{
    private IGroupReadRepository _groupRepository;

    public CategoryReadRepository(IServiceProvider provider, TransactionContext context, IServiceBus serviceBus)
        : base(context, serviceBus, Collections.CATEGORY)
        => this._groupRepository = provider.GetRequiredService<IGroupReadRepository>();

    public override async Task<CategoryModel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await base.FindByIdAsync(id, cancellationToken);
        var group = await this._groupRepository.FindByIdAsync(category.GroupId, cancellationToken);
        
        return category is null ? null : new CategoryModel(category.Id, category.Title, group);
    }
    
    public override async Task<IEnumerable<CategoryModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await base.GetAllAsync(cancellationToken);

        var groupIds = categories.Select(g => g.GroupId)
            .Distinct();
        
        var accounts = (await this._groupRepository.FindByIdAsync(groupIds, cancellationToken))
            .ToDictionary(a => a.Id);
        
        var finalCategories = categories.Select(category => accounts.TryGetValue(category.GroupId, out var group) 
            ? new CategoryModel(category.Id, category.Title, group) 
            : category);

        return finalCategories.ToList();
    }

    public async Task<ListCategoryResponse> ObtainsAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await this.GetAllAsync(cancellationToken);

        var categoryObjects = result.Select(category => new ListCategoryResponse.CategoryObject
        {
            Id = category.Id,
            Name = category.Title,
            Group = category.Group.Name,
            Type = (Contracts.Enums.EntryType) category.Group.Type
        });
        
        return new ListCategoryResponse(categoryObjects);
    }
}