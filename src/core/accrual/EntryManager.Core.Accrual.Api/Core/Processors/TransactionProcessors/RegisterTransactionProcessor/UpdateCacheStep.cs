using EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

public class UpdateCacheStep(IServiceProvider provider) : IRegisterTransactionProcessor
{
    private IRollupRepository _rollupRepository = provider.GetRequiredService<IRollupRepository>();
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task ProcessAsync(RegisterTransactionTransfer transfer, CancellationToken cancellationToken)
    {
        var rollupDay = transfer.Event.Transaction.OccurredAt;
        
        var rollupNotExists = !this._rollupRepository.Exists(rollupDay);

        if (rollupNotExists)
            await this._serviceBus.SendAsync(new InitializeRollupCommand(rollupDay), cancellationToken);
        
        return;
    }
    
    public void Dispose()
    {
        
    }
}