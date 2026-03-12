using EntryManager.Core.Accrual.Api.Application.Events.TransactionEvents;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.EventHandlers.TransactionEventHandlers;

public class ProcessAccountHandler(IServiceProvider provider) : IEventHandler<TransactionWasCommittedEvent>
{
    private IAccountRepository _accountRepository = provider.GetRequiredService<IAccountRepository>();
    private ICategoryReadRepository _categoryRepository = provider.GetRequiredService<ICategoryReadRepository>();
    private IGroupReadRepository _groupRepository = provider.GetRequiredService<IGroupReadRepository>();
    
    public async Task Handle(TransactionWasCommittedEvent notification, CancellationToken cancellationToken)
    {
        var category = await this._categoryRepository.FindByIdAsync(notification.CategoryId, cancellationToken);
        var group = await this._groupRepository.FindByIdAsync(category.GroupId, cancellationToken);

        var accId = group.AccountId;
        var delta = notification.Value;
        
        var account = await this._accountRepository.UpdateBalanceAsync(accId, delta, cancellationToken);
        
        // TODO: Add event sourcing to enable reliable rollback
    }

    public void Dispose()
    {
        
    }
}