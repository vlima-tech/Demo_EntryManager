using EntryManager.Core.Transaction.Contracts.Enums;

namespace EntryManager.Core.Transaction.Contracts.Responses.AccountResponses;

public class CreateAccountResponse
{
    public Guid AccountId { get; set; }

    public string Name { get; set; }

    public AccountStatus Status { get; set; }
}