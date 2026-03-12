using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Responses.TransactionResponses;

public class RegisterTransactionResponse
{
    public Guid TransactionId { get; set; }

    public string IdempotencyKey { get; set; }
    
    public string Title { get; set; }
    
    public int Value { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public CategoryObject Category { get; set; }
    
    public class CategoryObject
    {
        public Guid CategoryId { get; set; }
    
        public string Name { get; set; }
    
        public EntryType EntryType { get; set; }

        public string Group { get; set; }
    }
}