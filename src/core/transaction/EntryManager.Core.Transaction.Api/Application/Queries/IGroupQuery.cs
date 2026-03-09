using EntryManager.Core.Transaction.Contracts.Responses.GroupResponses;

namespace EntryManager.Core.Transaction.Api.Application.Queries;

public interface IGroupQuery
{
    /// <summary>
    /// Retrieves all groups.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    Task<ListGroupResponse> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously finds a group by name.
    /// </summary>
    /// <param name="groupName">The group name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    Task<FindGroupByNameResponse?> FindByNameAsync(string groupName, CancellationToken cancellationToken = default);
}