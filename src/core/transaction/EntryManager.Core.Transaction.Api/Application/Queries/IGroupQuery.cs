using EntryManager.Core.Transaction.Contracts.Responses.GroupResponses;

namespace EntryManager.Core.Transaction.Api.Application.Queries;

public interface IGroupQuery
{
    /// <summary>
    /// Retrieves all groups.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    Task<ListGroupResponse> ObtainsAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a group by its identification key.
    /// </summary>
    /// <param name="groupId">The group identification key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    Task<FindGroupByIdResponse?> ObtainsByIdAsync(Guid groupId, CancellationToken cancellationToken = default);
}