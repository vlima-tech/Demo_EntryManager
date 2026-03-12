using EntryManager.Core.Transaction.Contracts.Requests.GroupRequests;
using EntryManager.Core.Transaction.Contracts.Responses.GroupResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.GroupCommands;

public class CreateGroupCommand(CreateGroupRequest request) : Command<CreateGroupResponse?>
{
    public CreateGroupRequest Request { get; } = request;
}