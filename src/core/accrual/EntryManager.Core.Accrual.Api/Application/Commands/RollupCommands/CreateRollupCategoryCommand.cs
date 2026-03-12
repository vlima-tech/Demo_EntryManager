using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;

public class CreateRollupCategoryCommand(Guid categoryId) : Command
{
    public Guid CategoryId { get; } = categoryId;
}