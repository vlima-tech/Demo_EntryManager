using EntryManager.Core.Transaction.Contracts.Requests.TransactionRequests;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;

public class RegisterTransactionCommand(RegisterTransactionRequest request) : Command
{
    public RegisterTransactionRequest Request { get; } = request;
}