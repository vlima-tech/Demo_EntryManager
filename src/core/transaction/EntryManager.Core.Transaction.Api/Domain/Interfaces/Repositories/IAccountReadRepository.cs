using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Shared.Domain.Abstractions;

namespace EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;

public interface IAccountReadRepository : IBaseReadRepository<AccountModel, Guid>
{
    /// <summary>
    /// Asynchronously finds a model by name.
    /// </summary>
    /// <param name="accountName">The account name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    Task<AccountModel?> FindByNameAsync(string accountName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a model exists by its identification account name.
    /// </summary>
    /// <param name="accountName">The identification account name.</param>
    /// <returns>True if the model exists; otherwise, false.</returns>
    bool Exists(string accountName);
    
    /// <summary>
    /// Checks if a model not exists by its identification account name.
    /// </summary>
    /// <param name="accountName">The identification account name.</param>
    /// <returns>True if the model not exists; otherwise, false.</returns>
    bool NotExists(string accountName);
}