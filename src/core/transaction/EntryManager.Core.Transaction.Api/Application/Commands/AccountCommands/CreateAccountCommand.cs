using EntryManager.Core.Transaction.Contracts.Requests.AccountRequests;
using EntryManager.Core.Transaction.Contracts.Responses.AccountResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;

public class CreateAccountCommand(CreateAccountRequest request) : Command<CreateAccountResponse?>
{
    public CreateAccountRequest Request { get; } = request;
}