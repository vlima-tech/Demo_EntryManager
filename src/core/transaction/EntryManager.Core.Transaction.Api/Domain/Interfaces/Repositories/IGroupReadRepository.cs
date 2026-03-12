using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Shared.Domain.Abstractions;

namespace EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;

public interface IGroupReadRepository : IBaseReadRepository<GroupModel, Guid>
{
    /// <summary>
    /// Checks if a model exists by its identification group name.
    /// </summary>
    /// <param name="groupName">The identification group name.</param>
    /// <returns>True if the model exists; otherwise, false.</returns>
    bool Exists(string groupName);
    
    /// <summary>
    /// Checks if a model NOT exists by its identification group name.
    /// </summary>
    /// <param name="groupName">The identification group name.</param>
    /// <returns>True if the model exists; otherwise, false.</returns>
    bool NotExists(string groupName);
}