namespace EntryManager.Core.Transaction.Contracts.Objects;

public class TransactionObject
{
    public Guid TransactionId { get; set; }

    public Guid GroupId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid AccountId { get; set; }
    
    public string IdempotencyKey { get; set; }
    
    public string Title { get; set; }
    
    public int Value { get; set; }
    
    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public CategoryObject Category { get; set; }
}