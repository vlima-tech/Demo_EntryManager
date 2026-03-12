using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Transaction.Contracts.Events.AccountEvents;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Consumers;

public class AccountWasCreatedEventHandler(IServiceProvider provider) : IEventHandler<AccountWasCreatedEvent>
{
    private IAccountRepository _accountRepository = provider.GetRequiredService<IAccountRepository>();
    
    public async Task Handle(AccountWasCreatedEvent notification, CancellationToken cancellationToken)
    {
        var accObj = notification.Account;

        var accModel = new AccountModel(accObj.AccountId, accObj.Name, accObj.Balance, (Domain.Enums.AccountStatus)accObj.Status);

        await this._accountRepository.CreateAsync(accModel, cancellationToken);
    }

    public void Dispose()
    {
        this._accountRepository?.Dispose();
        this._accountRepository = null;
    }
}