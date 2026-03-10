namespace EntryManager.Core.Transaction.Contracts.Requests.TransactionRequests;

public class RegisterTransactionRequest
{
    public string Title { get; set; }

    public int Value { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime? Date { get; set; }
}