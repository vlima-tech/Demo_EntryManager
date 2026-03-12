using EntryManager.Core.Accrual.Api.Application.Events.TransactionEvents;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

public class CommitTransactionStep(IServiceProvider provider, IRegisterTransactionProcessor processor) : IRegisterTransactionProcessor
{
    private ITransactionRepository _transactionRepository = provider.GetRequiredService<ITransactionRepository>();
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task ProcessAsync(RegisterTransactionTransfer transfer, CancellationToken cancellationToken)
    {
        var transactionId = transfer.Event.Transaction.TransactionId;

        if (this._transactionRepository.Exists(transactionId))
        {
            return;
        }
        
        var obj = transfer.Event.Transaction;
        var transaction = new TransactionModel(transactionId, obj.CategoryId, obj.Title, obj.Value, obj.OccurredAt);
        
        await this._transactionRepository.CreateAsync(transaction, cancellationToken);

        if (this._store.HasNotifications())
            return;

        IEvent @event = new TransactionWasCommittedEvent(transaction.CategoryId, obj.OccurredAt, transaction.Value);
        await this._serviceBus.PublishAsync(@event, cancellationToken);

        if (this._store.HasNotifications())
        {
            // TODO: publish rollback event
            transaction.Rollback();
            
            return;
        }
        
        transaction.Commit();
        await this._transactionRepository.SaveAsync(transaction, cancellationToken);
        
        await processor.ProcessAsync(transfer, cancellationToken);
    }
    
    public void Dispose()
    {
        this._transactionRepository?.Dispose();
        this._transactionRepository = null;
    }
}