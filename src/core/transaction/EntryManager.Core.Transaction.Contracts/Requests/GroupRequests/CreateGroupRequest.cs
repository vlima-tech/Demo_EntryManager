using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Requests.GroupRequests;

public class CreateGroupRequest
{
    public Guid AccountId { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }

    public EntryType Type { get; set; }
}