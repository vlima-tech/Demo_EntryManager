using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Objects;

public class CategoryObject
{
    public Guid CategoryId { get; set; }
    
    public string Title { get; set; }
    
    public EntryType EntryType { get; set; }

    public GroupObject Group { get; set; }
}