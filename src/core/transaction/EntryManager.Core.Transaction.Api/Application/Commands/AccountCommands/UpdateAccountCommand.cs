using EntryManager.Core.Transaction.Contracts.Requests.AccountRequests;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;

public class UpdateAccountCommand(Guid accountId, UpdateAccountRequest request) : Command
{
    public Guid AccountId { get; } = accountId;
    
    public UpdateAccountRequest Request { get; } = request;
}