namespace EntryManager.Core.Transaction.Contracts.Requests.AccountRequests;

public class CreateAccountRequest
{
    public string Name { get; set; }

    public int Balance { get; set; }
}