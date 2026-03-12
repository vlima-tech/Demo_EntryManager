using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Application.Events.TransactionEvents;

public class TransactionWasRolledBackEvent(Guid categoryId, int ledgerId, int value) : Event(ExecutionMode.Immediate)
{
    public Guid CategoryId { get; } = categoryId;

    public int LedgerId { get; } = ledgerId;

    public int Value { get; } = value;
}