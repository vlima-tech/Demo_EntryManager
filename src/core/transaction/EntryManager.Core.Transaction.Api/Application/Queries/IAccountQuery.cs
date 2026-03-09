using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Contracts.Responses.AccountResponses;

namespace EntryManager.Core.Transaction.Api.Application.Queries;

public interface IAccountQuery
{
    /// <summary>
    /// Retrieves all accounts.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    Task<ListAccountResponse> ObtainsAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously finds a account by name.
    /// </summary>
    /// <param name="accountName">The account name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    Task<FindAccountByNameResponse?> FindByNameAsync(string accountName, CancellationToken cancellationToken = default);
}