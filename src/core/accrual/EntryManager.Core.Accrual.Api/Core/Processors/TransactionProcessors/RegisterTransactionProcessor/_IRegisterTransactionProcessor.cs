namespace EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

public interface IRegisterTransactionProcessor : IDisposable
{
    Task ProcessAsync(RegisterTransactionTransfer transfer, CancellationToken cancellationToken);
}