using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;

public class TransactionWasCreatedEvent : Event
{
    public TransactionObject Transaction { get; set; }
    
    public TransactionWasCreatedEvent() : base(ExecutionMode.Enqueue)
    { }

    public TransactionWasCreatedEvent(TransactionObject transaction) : base(ExecutionMode.Enqueue)
        => this.Transaction = transaction;
    
}