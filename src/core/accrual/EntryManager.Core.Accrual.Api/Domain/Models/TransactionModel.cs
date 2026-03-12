using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Accrual.Api.Domain.Models;

public class TransactionModel : BaseModel
{
    #region Properties
    
    public Guid CategoryId { get; private set; }
    
    public long LedgerId { get; private set; }

    public int EffectiveDay { get; private set; }
    
    public string Title { get; private set; }
    
    public int Value { get; private set; }
    
    public TransactionStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; set; }
    #endregion

    protected TransactionModel() { }
    
    public TransactionModel(Guid transactionId, Guid categoryId, string title, int value, DateTime settledAt)
    {
        this.Id = transactionId;
        this.CategoryId = categoryId;
        this.LedgerId = LedgerModel.GenerateId(categoryId, settledAt);
        this.EffectiveDay = int.Parse(settledAt.ToString("yyyyMMdd"));
        this.Title = title;
        this.Value = value;
        this.CreatedAt = DateTime.UtcNow;
        this.Status = TransactionStatus.Created;
    }
    
    public void Commit()
    {
        this.Status = TransactionStatus.Committed;
        this.UpdatedAt = DateTime.UtcNow;
    }
    
    public void Rollback()
    {
        this.Status = TransactionStatus.RolledBack;
        this.UpdatedAt = DateTime.UtcNow;
    }
}