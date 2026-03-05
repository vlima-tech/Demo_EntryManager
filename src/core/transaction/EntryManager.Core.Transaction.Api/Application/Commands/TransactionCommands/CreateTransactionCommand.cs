using EntryManager.Core.Transaction.Contracts.Requests.TransactionRequests;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;

public class CreateTransactionCommand(CreateTransactionRequest request) : Command(ExecutionMode.Enqueue)
{
    public CreateTransactionRequest Request { get; private set; } = request;
}