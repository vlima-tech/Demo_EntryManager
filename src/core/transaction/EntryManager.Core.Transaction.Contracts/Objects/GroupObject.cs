using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Objects;

public class GroupObject
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public GroupType Type { get; set; }
    
    public string Account { get; set; }
}