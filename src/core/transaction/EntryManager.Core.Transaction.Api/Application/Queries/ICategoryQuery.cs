using EntryManager.Core.Transaction.Contracts.Responses.CategoryResponses;

namespace EntryManager.Core.Transaction.Api.Application.Queries;

public interface ICategoryQuery
{
    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    Task<ListCategoryResponse> ObtainsAllAsync(CancellationToken cancellationToken = default);
}