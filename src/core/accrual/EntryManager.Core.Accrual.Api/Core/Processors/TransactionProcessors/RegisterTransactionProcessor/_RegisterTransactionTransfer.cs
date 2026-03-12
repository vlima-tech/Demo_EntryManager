using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;

namespace EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

public class RegisterTransactionTransfer(TransactionWasCreatedEvent @event)
{
    public TransactionWasCreatedEvent Event { get; } = @event;
}