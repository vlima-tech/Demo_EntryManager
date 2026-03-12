using EntryManager.Core.Transaction.Contracts.Requests.CategoryRequests;
using EntryManager.Core.Transaction.Contracts.Responses.CategoryResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.CategoryCommands;

public class CreateCategoryCommand(CreateCategoryRequest request) : Command<CreateCategoryResponse>
{
    public CreateCategoryRequest Request { get; } = request;
}