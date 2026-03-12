using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Objects;

public class AccountObject
{
    public Guid AccountId { get; set; }
    
    public string Name { get; set; }
    
    public int Balance { get; set; }

    public AccountStatus Status { get; set; }
}