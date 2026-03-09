namespace EntryManager.Core.Transaction.Contracts.Requests.CategoryRequests;

public class CreateCategoryRequest
{
    public Guid GroupId { get; set; }
    
    public string Name { get; set; }
}