using EntryManager.Shared.Domain.Abstractions.Objects;

namespace EntryManager.Shared.Domain.Abstractions;

public interface IBaseRepository<TModel, in TKey> : IBaseReadRepository<TModel, TKey>
    where TModel : IdentifiedObject<TKey>
{
    /// <summary>
    /// Inserts a model into the database.
    /// </summary>
    /// <param name="model">The model instance to insert.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the model was successfully inserted; otherwise, false.</returns>
    Task<bool> CreateAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a collection of models into the database.
    /// </summary>
    /// <param name="models">The collection of models to insert.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts multiple models into the database using a params array.
    /// </summary>
    /// <param name="models">The model instances to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(params TModel[] models);
    
    /// <summary>
    /// Updates an existing model in the database by replacing the entire document.
    /// </summary>
    /// <param name="model">The model instance to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the model was found and the update operation was acknowledged; otherwise, false.</returns>
    Task<bool> UpdateAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a collection of models using bulk operations.
    /// </summary>
    /// <param name="models">The collection of models to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple models in the database using a params array.
    /// </summary>
    /// <param name="models">The model instances to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(params TModel[] models);
    
    /// <summary>
    /// Saves the model by inserting it if it doesn't exist or updating it if it does.
    /// </summary>
    /// <param name="model">The model to save.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the operation was acknowledged; otherwise, false.</returns>
    Task<bool> SaveAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a collection of models by inserting new ones or updating existing ones.
    /// This operation uses BulkWrite for atomic upserts, reducing database round-trips.
    /// </summary>
    /// <param name="models">The collection of models to save.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task SaveRangeAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves multiple models in the database using a params array.
    /// </summary>
    /// <param name="models">The model instances to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveRangeAsync(params TModel[] models);
    
    /// <summary>
    /// Removes a model from the database by its identification key.
    /// </summary>
    /// <param name="id">The identification key of the model.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the model was successfully tracked for removal; otherwise, false.</returns>
    Task<bool> RemoveAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple models from the database using a params array of IDs.
    /// </summary>
    /// <param name="ids">The identification keys to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(params TKey[] ids);
    
    /// <summary>
    /// Removes multiple models from the database by their identification keys.
    /// </summary>
    /// <param name="ids">The collection of identification keys.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task RemoveAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a specific model instance from the database.
    /// </summary>
    /// <param name="model">The model to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the removal was successfully tracked; otherwise, false.</returns>
    Task<bool> RemoveAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a collection of model instances from the database.
    /// </summary>
    /// <param name="models">The collection of models to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task RemoveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple model instances from the database.
    /// </summary>
    /// <param name="models">The model instances to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(params TModel[] models);
    
    /// <summary>
    /// Persists all tracked changes to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the changes were successfully committed; otherwise, false.</returns>
    Task<bool> CommitAsync(CancellationToken cancellationToken = default);
}