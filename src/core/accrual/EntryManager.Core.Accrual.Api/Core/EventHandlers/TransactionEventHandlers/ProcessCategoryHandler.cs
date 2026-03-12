using EntryManager.Core.Accrual.Api.Application.Events.TransactionEvents;
using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.EventHandlers.TransactionEventHandlers;

public class ProcessCategoryHandler(IServiceProvider provider) : IEventHandler<TransactionWasCommittedEvent>
{
    private ILedgerRepository _ledgerRepository = provider.GetRequiredService<ILedgerRepository>();
    
    public async Task Handle(TransactionWasCommittedEvent @event, CancellationToken cancellationToken)
    {
        var ledger = new LedgerModel(@event.CategoryId, @event.EffectedAt, ReferenceType.Category, @event.Value);
        
        await this._ledgerRepository.SaveAsync(ledger, cancellationToken);
        
        // TODO: Add event sourcing to enable reliable rollback
    }

    public void Dispose()
    {
        this._ledgerRepository?.Dispose();
        this._ledgerRepository = null;
    }
}