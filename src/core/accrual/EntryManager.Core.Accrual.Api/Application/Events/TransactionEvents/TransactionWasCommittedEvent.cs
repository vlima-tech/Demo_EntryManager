using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Application.Events.TransactionEvents;

public class TransactionWasCommittedEvent(Guid categoryId, DateTime effectedAt, int value) : Event()
{
    public Guid CategoryId { get; } = categoryId;

    public DateTime EffectedAt { get; } = effectedAt;

    public int Value { get; } = value;
}