using EntryManager.Core.Transaction.Api.Application.Commands.CategoryCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.CategoryCommandHandlers;

public class UpdateCategoryCommandHandler(IServiceProvider provider) : ICommandHandler<UpdateCategoryCommand>
{
    private ICategoryRepository _categoryRepository = provider.GetRequiredService<ICategoryRepository>();
    private INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task<bool> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        var category = await this._categoryRepository.FindByIdAsync(command.CategoryId, cancellationToken);
        
        category.ChangeName(request.Name);
        
        await this._categoryRepository.UpdateAsync(category, cancellationToken);
        
        return !this._store.HasNotifications();
    }

    public void Dispose()
    {
        
    }
}