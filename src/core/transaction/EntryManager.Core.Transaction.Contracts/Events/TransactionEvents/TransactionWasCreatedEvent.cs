using EntryManager.Core.Transaction.Contracts.Requests.TransactionRequests;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;

public class TransactionWasCreatedEvent(CreateTransactionRequest request) : Event(ExecutionMode.Enqueue)
{
    public readonly CreateTransactionRequest Request = request;
}