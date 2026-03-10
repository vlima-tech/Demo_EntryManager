using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Transaction.Api.Domain.Models;

public class TransactionModel : BaseModel
{
    #region Properties
    
    public Guid CategoryId { get; private set; }
    
    public string IdempotencyKey { get; private set; }
    
    public string Title { get; private set; }
    
    public int Value { get; private set; }
    
    public DateTime RegisteredAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public CategoryModel Category { get; private set; }
    
    #endregion
    
    #region Constructors

    public TransactionModel()
    { }
    
    public TransactionModel(string idempotencyKey, string title, int value, DateTime registeredAt, CategoryModel category)
    {
        this.CategoryId = category.Id;
        this.IdempotencyKey = idempotencyKey;
        this.Title = title;
        this.Value = category.EntryType == EntryType.Expense ? value * -1 : value;
        this.RegisteredAt = registeredAt;
        this.CreatedAt = DateTime.UtcNow;
        this.Category = category;
    }   
    
    #endregion
}