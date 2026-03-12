using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Accrual.Api.Domain.Models;

public class GroupModel : BaseModel
{
    #region Properties

    public Guid AccountId { get; private set; }
    
    public string Name { get; private set; }
    
    public string Description { get; private set; }

    public EntryType Type { get; private set; }
    
    #endregion
    
    #region Constructors

    protected GroupModel() { }

    public GroupModel(Guid groupId, Guid accountId, string name, string description, EntryType type)
    {
        this.Id = groupId;
        this.AccountId = accountId;
        this.Name = name?.Trim();
        this.Description = description?.Trim();
        this.Type = type;
    }
    
    #endregion
    
    public static GroupModel Create(Guid groupId, Guid accountId, string name, string description, EntryType type)
        => new(groupId, accountId, name, description, type);
    
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