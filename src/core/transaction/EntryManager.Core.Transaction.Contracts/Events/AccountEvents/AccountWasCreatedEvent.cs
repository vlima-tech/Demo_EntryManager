using EntryManager.Core.Transaction.Contracts.Enums;
using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Contracts.Events.AccountEvents;

public class AccountWasCreatedEvent : Event
{
    public AccountObject Account { get; set; }

    protected AccountWasCreatedEvent() : base(ExecutionMode.Enqueue) { }
    
    public AccountWasCreatedEvent(AccountObject account) : base(ExecutionMode.Enqueue)
        => this.Account = account;

    public AccountWasCreatedEvent(Guid accountId, string name, int balance, AccountStatus status) : base(ExecutionMode.Enqueue)
        => this.Account = new AccountObject { AccountId = accountId, Name = name, Balance = balance, Status = status };
}