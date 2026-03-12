using EntryManager.Core.Accrual.Api.Domain.Interfaces;

namespace EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

public class CheckAccountStep(IServiceProvider provider, IRegisterTransactionProcessor processor) : IRegisterTransactionProcessor
{
    private IAccountRepository _accountRepository = provider.GetRequiredService<IAccountRepository>();
    
    public async Task ProcessAsync(RegisterTransactionTransfer transfer, CancellationToken cancellationToken)
    {
        var accountId = transfer.Event.Transaction.AccountId;

        if(this._accountRepository.Exists(accountId))
        {
            await processor.ProcessAsync(transfer, cancellationToken);
            return;
        }
        
        // TODO: Fallback and notification
    }
    
    public void Dispose()
    {
        this._accountRepository?.Dispose();
        this._accountRepository = null;
    }
}