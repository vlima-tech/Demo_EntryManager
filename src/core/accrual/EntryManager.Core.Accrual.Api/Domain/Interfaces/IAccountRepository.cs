using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Domain.Abstractions;

namespace EntryManager.Core.Accrual.Api.Domain.Interfaces;

public interface IAccountRepository : IBaseRepository<AccountModel, Guid>, IAccountReadRepository
{
    /// <summary>
    /// Performs an atomic update of the account balance.
    /// </summary>
    /// <param name="accountId">The account unique identifier.</param>
    /// <param name="amount">
    /// The amount to be applied to the current balance. 
    /// Use positive values for Credits and negative values for Debits.
    /// </param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// Returns <c>true</c> if the account was found and the balance successfully updated; 
    /// otherwise, <c>false</c>.
    /// </returns>
    Task<bool> UpdateBalanceAsync(Guid accountId, long amount, CancellationToken cancellationToken = default);
}