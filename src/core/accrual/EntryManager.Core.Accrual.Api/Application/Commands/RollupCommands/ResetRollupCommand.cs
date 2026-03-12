using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;

public class ResetRollupCommand(DateTime rollupDay) : Command
{
    public DateTime RollupDay { get; } = rollupDay;
}