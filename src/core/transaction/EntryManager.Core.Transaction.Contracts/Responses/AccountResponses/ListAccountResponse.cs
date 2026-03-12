using EntryManager.Core.Transaction.Contracts.Objects;

namespace EntryManager.Core.Transaction.Contracts.Responses.AccountResponses;

public class ListAccountResponse(IEnumerable<AccountObject> accounts) : List<AccountObject>(accounts);