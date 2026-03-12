using EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.CommandHandlers.RollupCommandHandlers;

public class CreateRollupCategoryCommandHandler(IServiceProvider provider) : ICommandHandler<CreateRollupCategoryCommand>
{
    public Task<bool> Handle(CreateRollupCategoryCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public void Dispose()
    {
        
    }
}