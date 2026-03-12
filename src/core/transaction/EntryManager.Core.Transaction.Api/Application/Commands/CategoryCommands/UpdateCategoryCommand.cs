using EntryManager.Core.Transaction.Contracts.Requests.CategoryRequests;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Application.Commands.CategoryCommands;

public class UpdateCategoryCommand(Guid categoryId, UpdateCategoryRequest request) : Command
{
    public Guid CategoryId { get; } = categoryId;
    
    public UpdateCategoryRequest Request { get; } = request;
}