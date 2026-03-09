using EntryManager.Core.Transaction.Api.Application.Commands.GroupCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.GroupCommandHandlers;

public class UpdateGroupCommandHandler(IServiceProvider provider) : ICommandHandler<UpdateGroupCommand>
{
    private IGroupRepository _groupRepository = provider.GetRequiredService<IGroupRepository>();
    private INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task<bool> Handle(UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        var group = await this._groupRepository.FindByIdAsync(command.GroupId, cancellationToken);
        
        group.ChangeName(request.Name, request.Description);
        
        await this._groupRepository.UpdateAsync(group, cancellationToken);

        return !this._store.HasNotifications();
    }

    public void Dispose()
    {
        this._groupRepository?.Dispose();
        this._groupRepository = null;
    }
}