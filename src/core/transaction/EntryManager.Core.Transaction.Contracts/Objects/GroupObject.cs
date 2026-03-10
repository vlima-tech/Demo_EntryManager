using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Objects;

public class GroupObject
{
    public Guid GroupId { get; set; }

    public string Name { get; set; }

    public EntryType Type { get; set; }
    
    public string Account { get; set; }
}