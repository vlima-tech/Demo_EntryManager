using EntryManager.Core.Accrual.Api.Application.Events.TransactionEvents;
using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.EventHandlers.TransactionEventHandlers;

public class ProcessGroupHandler(IServiceProvider provider) : IEventHandler<TransactionWasCommittedEvent>
{
    private ICategoryReadRepository _categoryRepository = provider.GetRequiredService<ICategoryReadRepository>();
    private ILedgerRepository _ledgerRepository = provider.GetRequiredService<ILedgerRepository>();
    
    public async Task Handle(TransactionWasCommittedEvent @event, CancellationToken cancellationToken)
    {
        var category = await this._categoryRepository.FindByIdAsync(@event.CategoryId, cancellationToken);
        
        var ledger = new LedgerModel(category.GroupId, @event.EffectedAt, ReferenceType.Group, @event.Value);

        await this._ledgerRepository.SaveAsync(ledger, cancellationToken);
        
        // TODO: Add event sourcing to enable reliable rollback
    }

    public void Dispose()
    {
        this._categoryRepository?.Dispose();
        this._categoryRepository = null;
        
        this._ledgerRepository?.Dispose();
        this._ledgerRepository = null;
    }
}