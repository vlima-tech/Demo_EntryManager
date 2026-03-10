using System.Linq.Expressions;
using EntryManager.Shared.Domain.Abstractions.Objects;

namespace EntryManager.Shared.Domain.Abstractions;

public interface IBaseReadRepository<TModel, in TKey> : IDisposable 
    where TModel : IdentifiedObject<TKey>
{
    /// <summary>
    /// Checks if a model exists by its identification key.
    /// </summary>
    /// <param name="id">The identification key.</param>
    /// <returns>True if the model exists; otherwise, false.</returns>
    bool Exists(TKey id);

    /// <summary>
    /// Checks if the specified model exists in the repository based on its <see cref="TKey"/>.
    /// </summary>
    /// <param name="model">The model instance to verify.</param>
    /// <returns>True if a model with the same identification key exists; otherwise, false.</returns>
    bool Exists(TModel model);

    /// <summary>
    /// Checks if a model exists based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <returns>True if any model satisfies the condition; otherwise, false.</returns>
    bool Exists(Expression<Func<TModel, bool>> predicate);
    
    /// <summary>
    /// Checks if a model not exists by its identification key.
    /// </summary>
    /// <param name="id">The identification key.</param>
    /// <returns>True if the model not exists; otherwise, false.</returns>
    bool NotExists(TKey id);

    /// <summary>
    /// Checks if the specified model not exists in the repository based on its <see cref="TKey"/>.
    /// </summary>
    /// <param name="model">The model instance to verify.</param>
    /// <returns>True if a model with the same identification key not exists; otherwise, false.</returns>
    bool NotExists(TModel model);

    /// <summary>
    /// Checks if a model not exists based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <returns>True if any model not satisfies the condition; otherwise, false.</returns>
    bool NotExists(Expression<Func<TModel, bool>> predicate);
    
    /// <summary>
    /// Asynchronously finds a model by its <see cref="TKey"/>.
    /// </summary>
    /// <param name="id">The identification key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    Task<TModel?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously finds a collection of models by their <see cref="TKey"/> identifiers.
    /// </summary>
    /// <param name="ids">The collection of identification keys to search for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    Task<IEnumerable<TModel>> FindByIdAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Filters a sequence of models based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection of models that match the predicate. An empty list if nothing is found.</returns>
    Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Filters a sequence of models with pagination support.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="filter">The pagination parameters to limit the result set and define the page offset.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection of models. An empty list if nothing is found.</returns>
    Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, PaginationFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously filters a sequence of models using a delegate to configure pagination.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="filter">A delegate to configure the pagination and search options.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of models. An empty list is returned if no models match the predicate.</returns>
    Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, Action<PaginationFilter> filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Provides an <see cref="IQueryable{TModel}"/> for advanced LINQ queries that are translated to MongoDB aggregation pipelines.
    /// </summary>
    /// <returns>An <see cref="IQueryable{TModel}"/> that allows further LINQ composition and deferred execution on the server side.</returns>
    IQueryable<TModel> Query();

    /// <summary>
    /// Provides a filtered <see cref="IQueryable{TModel}"/> based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <returns>A filtered queryable for further composition.</returns>
    IQueryable<TModel> Query(Expression<Func<TModel, bool>> predicate);

    /// <summary>
    /// Counts the total number of models in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>The total number of models that match the criteria, or 0 if none are found.</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts models that satisfy a specific condition.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>The total number of models that match the criteria, or 0 if none are found.</returns>
    Task<long> CountAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all models. Caution: Not recommended for large datasets.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all models using pagination.
    /// </summary>
    /// <param name="filter">The pagination parameters to limit the result set and define the page offset.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    Task<IEnumerable<TModel>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default);
}