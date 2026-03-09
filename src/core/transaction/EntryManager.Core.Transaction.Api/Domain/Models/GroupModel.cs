using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Transaction.Api.Domain.Models;

public class GroupModel : BaseModel
{
    #region Properties

    public Guid AccountId { get; private set; }
    
    public string Name { get; private set; }
    
    public string Description { get; private set; }

    public GroupType Type { get; private set; }

    public AccountModel Account { get; private set; }
    
    #endregion
    
    #region Constructors

    public GroupModel() { }

    public GroupModel(string name, string description, GroupType type, AccountModel account) : this()
    {
        this.AccountId = account.Id;
        this.Name = name?.Trim();
        this.Description = description?.Trim();
        this.Type = type;
        this.Account = account;
    }
    
    public GroupModel(Guid groupId, string name, string description, GroupType type, AccountModel account)
    : this(name, description, type, account)
    {
        this.Id = groupId;
    }
    
    #endregion

    public static GroupModel Create(string name, string description, GroupType type, AccountModel accountModel)
        => new(name, description, type, accountModel);
    
    public static GroupModel Create(Guid groupId, string name, string description, GroupType type, AccountModel accountModel)
        => new(groupId, name, description, type, accountModel);
    
    public void ChangeName(string groupName)
    {
        if(string.IsNullOrEmpty(groupName?.Trim()))
            return;
        
        this.Name = groupName.Trim();
    }
    
    public void ChangeName(string groupName, string description)
    {
        if(string.IsNullOrEmpty(groupName?.Trim()))
            return;
        
        this.Name = groupName.Trim();
        this.Description = description?.Trim() ?? string.Empty;
    }
}