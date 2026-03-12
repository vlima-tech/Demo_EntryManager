using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Accrual.Api.Domain.Models;

public class AccountModel : BaseModel
{
    public string Name { get; private set; }
    
    public int Balance { get; private set; }

    public AccountStatus Status { get; private set; }

    public AccountModel(Guid id, string name, int balance, AccountStatus status)
    {
        base.Id = id;
        this.Name = name;
        this.Balance = balance;
        this.Status = status;
    }

    public static AccountModel Create(Guid id, string name, int balance, AccountStatus status)
        => new(id, name, balance, status);
    
    public void ChangeName(string newName) => this.Name = newName;
    
    public void Deposit(int amount)
    {
        if(amount < 0) return;
        
        this.Balance += amount;
    }

    public void Reset()
    {
        this.Balance = 0;
        this.Status = AccountStatus.Reseted;
    }
    
    public void Activate() => this.Status = AccountStatus.Active;
    
    public void Deactivate() => this.Status = AccountStatus.Inactive;
}