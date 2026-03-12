using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;

public class InitializeRollupCommand(DateTime rollupDay) : Command
{
    public DateTime RollupDay { get; } = rollupDay;
}