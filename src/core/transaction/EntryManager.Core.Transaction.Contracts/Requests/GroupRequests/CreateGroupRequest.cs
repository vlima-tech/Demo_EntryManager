using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Requests.GroupRequests;

public class CreateGroupRequest
{
    public string Name { get; set; }
    
    public string Description { get; set; }

    public EntryType Type { get; set; }

    public string AccountName { get; set; }
}