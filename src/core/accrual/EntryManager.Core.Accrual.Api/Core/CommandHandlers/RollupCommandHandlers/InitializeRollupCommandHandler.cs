using EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.CommandHandlers.RollupCommandHandlers;

public class InitializeRollupCommandHandler(IServiceProvider provider) : ICommandHandler<InitializeRollupCommand>
{
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task<bool> Handle(InitializeRollupCommand command, CancellationToken cancellationToken)
    {
        return !this._store.HasNotifications();
    }

    public void Dispose()
    {
        
    }
}