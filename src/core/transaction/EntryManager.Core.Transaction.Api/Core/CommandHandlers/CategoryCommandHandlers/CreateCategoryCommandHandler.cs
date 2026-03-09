using EntryManager.Core.Transaction.Api.Application.Commands.CategoryCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Core.Transaction.Contracts.Responses.CategoryResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.CategoryCommandHandlers;

public class CreateCategoryCommandHandler(IServiceProvider provider) : ICommandHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    private ICategoryRepository _categoryRepository = provider.GetRequiredService<ICategoryRepository>();
    private IGroupReadRepository _groupRepository = provider.GetRequiredService<IGroupReadRepository>();
    
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        var group = await this._groupRepository.FindByIdAsync(request.GroupId, cancellationToken);
        var category = new CategoryModel(request.Name, group);
        
        await this._categoryRepository.CreateAsync(category, cancellationToken);

        return new CreateCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Group = new GroupObject
            {
                Id = group.Id,
                Name = group.Name,
                Type = (Contracts.Enums.GroupType)group.Type,
                Account = group.Account.Name
            }
        };
    }

    public void Dispose()
    {
        this._categoryRepository?.Dispose();
        this._categoryRepository = null;
        
        this._groupRepository?.Dispose();
        this._groupRepository = null;
    }
}