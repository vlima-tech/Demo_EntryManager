using EntryManager.Core.Transaction.Contracts.Requests.GroupRequests;
using EntryManager.Core.Transaction.Contracts.Responses.GroupResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.GroupCommands;

public class UpdateGroupCommand(Guid groupId, UpdateGroupRequest request) : Command
{
    public Guid GroupId { get; } = groupId;
    
    public UpdateGroupRequest Request { get; } = request;
}