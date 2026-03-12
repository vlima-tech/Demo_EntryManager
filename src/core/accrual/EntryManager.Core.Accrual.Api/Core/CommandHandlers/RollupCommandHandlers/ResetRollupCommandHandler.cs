using EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;
using EntryManager.Core.Accrual.Api.Application.Validators.RollupValidators;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.CommandHandlers.RollupCommandHandlers;

public class ResetRollupCommandHandler(IServiceProvider provider) : ICommandHandler<ResetRollupCommand>
{
    private IRollupRepository _rollupRepository = provider.GetRequiredService<IRollupRepository>();
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    
    public async Task<bool> Handle(ResetRollupCommand command, CancellationToken cancellationToken)
    {
        var rollupDay = command.RollupDay;

        if (this._rollupRepository.Exists(rollupDay))
        {
            await this._rollupRepository.RemoveAsync(rollupDay, cancellationToken);
            return true;
        }
        
        await this._serviceBus.SendAsync(new InitializeRollupCommand(rollupDay), cancellationToken);

        if (this._rollupRepository.Exists(rollupDay))
            return true;
        
        var notification = new Warning(code: RollupValidationErrors.RollupNotExists.ErrorCode, RollupValidationErrors.RollupNotExists.ErrorMessage);
        await this._serviceBus.PublishAsync(notification, cancellationToken);
        
        return true;
    }

    public void Dispose()
    {
        this._rollupRepository = null;
    }
}