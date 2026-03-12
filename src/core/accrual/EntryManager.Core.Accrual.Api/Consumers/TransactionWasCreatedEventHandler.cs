using EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;
using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Consumers;

public class TransactionWasCreatedEventHandler(IServiceProvider provider) : IEventHandler<TransactionWasCreatedEvent>
{
    private IRegisterTransactionProcessor _processor = provider.GetRequiredService<IRegisterTransactionProcessor>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task Handle(TransactionWasCreatedEvent notification, CancellationToken cancellationToken)
    {
        var transfer = new RegisterTransactionTransfer(notification);
        
        await this._processor.ProcessAsync(transfer, cancellationToken);
        
        if(!this._store.HasNotifications())
            return;
        
        // TODO: DLQ and retry
    }

    public void Dispose()
    {
        this._processor?.Dispose();
        this._processor = null;
    }
}