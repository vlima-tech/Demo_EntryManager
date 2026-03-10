namespace EntryManager.Core.Transaction.Contracts.Objects;

public class TransactionObject
{
    public Guid TransactionId { get; set; }
    
    public string IdempotencyKey { get; set; }
    
    public string Title { get; set; }
    
    public int Value { get; set; }
    
    public DateTime RegisteredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public CategoryObject Category { get; set; }
}