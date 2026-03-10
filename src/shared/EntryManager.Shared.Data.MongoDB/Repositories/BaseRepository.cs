using System.Linq.Expressions;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Domain.Abstractions;
using EntryManager.Shared.Domain.Abstractions.Objects;
using MongoDB.Driver;

namespace EntryManager.Shared.Data.MongoDB.Repositories;

public class BaseRepository<TModel, TKey> : IBaseRepository<TModel, TKey> where TModel : IdentifiedObject<TKey>
{
    private readonly IServiceBus ServiceBus;
    protected IMongoCollection<TModel> Db { get; private set; }
    private IBaseReadRepository<TModel, TKey> _readRepository { get; set; }

    public BaseRepository(DbContext context, IServiceBus serviceBus, IBaseReadRepository<TModel, TKey> readRepository)
    {
        this.ServiceBus = serviceBus;
        this.Db = context.Set<TModel>();
        this._readRepository = readRepository;
    }

    public BaseRepository(DbContext context, IServiceBus serviceBus, IBaseReadRepository<TModel, TKey> readRepository, string collection)
    {
        ServiceBus = serviceBus;
        Db = context.Set<TModel>(collection);
        _readRepository = readRepository;
    }

    /// <summary>
    /// Inserts a model into the database.
    /// </summary>
    /// <param name="model">The model instance to insert.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the model was successfully inserted; otherwise, false.</returns>
    public virtual async Task<bool> CreateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        try
        { 
            await this.Db.InsertOneAsync(model, cancellationToken);
            
            return true;
        }
        catch(Exception e)
        {
            var msg = $"An error occurred while inserting a model of type {typeof(TModel).Name}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }

        return false;
    }

    /// <summary>
    /// Inserts a collection of models into the database.
    /// </summary>
    /// <param name="models">The collection of models to insert.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task CreateAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        try
        {
            if (models == null || !models.Any()) return;

            await this.Db.InsertManyAsync(models, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while inserting a collection of models of type {typeof(TModel).Name}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }
    }

    /// <summary>
    /// Inserts multiple models into the database using a params array.
    /// </summary>
    /// <param name="models">The model instances to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task CreateAsync(params TModel[] models)
        => this.CreateAsync(models.AsEnumerable());

    /// <summary>
    /// Updates an existing model in the database by replacing the entire document.
    /// </summary>
    /// <param name="model">The model instance to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the model was found and the update operation was acknowledged; otherwise, false.</returns>
    public virtual async Task<bool> UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await this.Db.ReplaceOneAsync(
                Builders<TModel>.Filter.Eq(m => m.Id, model.Id),
                model,
                new ReplaceOptions { IsUpsert = false }, 
                cancellationToken);

            return result.MatchedCount > 0;
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while updating a model of type {typeof(TModel).Name} with ID: {model.Id}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }

        return false;
    }

    /// <summary>
    /// Updates a collection of models using bulk operations.
    /// </summary>
    /// <param name="models">The collection of models to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task UpdateAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        if (models == null || !models.Any()) return;
        
        try
        {
            var updates = models.Select(model => 
                new ReplaceOneModel<TModel>(Builders<TModel>.Filter.Eq(m => m.Id, model.Id), model)
                {
                    IsUpsert = false
                }).ToList();

            var result = await this.Db.BulkWriteAsync(updates, new BulkWriteOptions { IsOrdered = false }, cancellationToken);

            if (result.MatchedCount < updates.Count)
            {
                var msg = $"Bulk update finished. Matched: {result.MatchedCount} of {updates.Count}. Some models were not found.";
                await this.ServiceBus.PublishAsync(new Log(msg), cancellationToken);
            }  
        }
        catch (Exception e)
        {
            var msg = $"An error occurred during bulk update of type {typeof(TModel).Name}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }
    }

    /// <summary>
    /// Updates multiple models in the database using a params array.
    /// </summary>
    /// <param name="models">The model instances to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task UpdateAsync(params TModel[] models)
        => UpdateAsync(models.AsEnumerable());

    /// <summary>
    /// Saves the model by inserting it if it doesn't exist or updating it if it does.
    /// This operation is atomic on the database level.
    /// </summary>
    /// <param name="model">The model to save.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the operation was acknowledged; otherwise, false.</returns>
    public virtual async Task<bool> SaveAsync(TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Db.ReplaceOneAsync(
                Builders<TModel>.Filter.Eq(m => m.Id, model.Id),
                model,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);

            return result.UpsertedId != null || result.MatchedCount > 0;
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while saving (upsert) model of type {typeof(TModel).Name} with ID: {model.Id}.";
            await ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }
        
        return false;
    }
    
    /// <summary>
    /// Saves a collection of models by inserting new ones or updating existing ones.
    /// This operation uses BulkWrite for atomic upserts, reducing database round-trips.
    /// </summary>
    /// <param name="models">The collection of models to save.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public virtual async Task SaveRangeAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        if (models == null || !models.Any()) return;

        try
        {
            var operations = models.Select(model => 
                new ReplaceOneModel<TModel>(Builders<TModel>.Filter.Eq(m => m.Id, model.Id), model)
            {
                IsUpsert = true
            }).ToList();

            await this.Db.BulkWriteAsync(
                operations, 
                new BulkWriteOptions
                {
                    IsOrdered = false
                }, 
                cancellationToken);
        }
        catch (Exception e)
        {
            var msg = $"An error occurred during bulk save (upsert) of type {typeof(TModel).Name}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }
    }
    
    /// <summary>
    /// Saves multiple models in the database using a params array.
    /// </summary>
    /// <param name="models">The model instances to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task SaveRangeAsync(params TModel[] models)
    {
        if (models == null || models.Length == 0)
            return Task.CompletedTask;

        return SaveRangeAsync(models.AsEnumerable());
    }

    /// <summary>
    /// Removes a model from the database by its identification key.
    /// </summary>
    /// <param name="id">The identification key of the model to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the model was found and removed; otherwise, false.</returns>
    public virtual async Task<bool> RemoveAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await this.Db.DeleteOneAsync(
                Builders<TModel>.Filter.Eq(m => m.Id, id), 
                cancellationToken);

            return result.DeletedCount > 0;
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while removing a model of type {typeof(TModel).Name} with ID: {id}.";
        
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        
            return false;
        }
    }

    /// <summary>
    /// Removes multiple models from the database using a params array of IDs.
    /// </summary>
    /// <param name="ids">The identification keys to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task RemoveAsync(params TKey[] ids)
    {
        if (ids == null || ids.Length == 0)
            return Task.CompletedTask;

        return RemoveAsync(ids.AsEnumerable());
    }

    /// <summary>
    /// Removes multiple models from the database by their identification keys.
    /// </summary>
    /// <param name="ids">The collection of identification keys.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public virtual async Task RemoveAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            if (ids == null || !ids.Any()) return;

            var filter = Builders<TModel>.Filter.In(m => m.Id, ids);
        
            var result = await this.Db.DeleteManyAsync(filter, cancellationToken);

            if (result.DeletedCount < ids.Count())
            {
                var msg = $"Bulk remove finished. Deleted: {result.DeletedCount} of {ids.Count()}. Some IDs were not found.";
                await this.ServiceBus.PublishAsync(new Log(msg), cancellationToken);
            }
        }
        catch (Exception e)
        {
            var msg = $"An error occurred during bulk removal of type {typeof(TModel).Name}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }
    }

    /// <summary>
    /// Removes a model instance from the database using its identification key.
    /// </summary>
    /// <param name="model">The model instance to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the model was found and successfully removed; otherwise, false.</returns>
    public virtual async Task<bool> RemoveAsync(TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            return await this.RemoveAsync(model.Id, cancellationToken);
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while removing model instance of type {typeof(TModel).Name}.";
            await ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        
            return false;
        }
    }
    
    /// <summary>
    /// Removes multiple model instances from the database.
    /// </summary>
    /// <param name="models">The model instances to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task RemoveAsync(IEnumerable<TModel> models, CancellationToken cancellationToken = default)
    {
        if (models == null || models.Count() == 0)
            return Task.CompletedTask;

        return this.RemoveAsync(models.Select(m => m.Id), cancellationToken);
    }

    /// <summary>
    /// Removes multiple model instances from the database.
    /// </summary>
    /// <param name="models">The model instances to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task RemoveAsync(params TModel[] models)
    {
        if (models == null || models.Length == 0)
            return Task.CompletedTask;

        return this.RemoveAsync(models.Select(m => m.Id));
    }

    /// <summary>
    /// Placeholder for Unit of Work pattern compatibility. 
    /// In this MongoDB implementation, changes are persisted immediately during method execution, 
    /// therefore a manual commit is not required.
    /// </summary>
    /// <returns>Always returns <strong>true</strong> as operations are atomic and immediate.</returns>
    public virtual bool Commit() => true;

    /// <summary>
    /// Asynchronously acknowledges the completion of operations. 
    /// Since this repository does not use a deferred Unit of Work (UoW), 
    /// all changes are already committed to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that immediately resolves to <strong>true</strong>.</returns>
    public virtual Task<bool> CommitAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);
    
    /// <summary>
    /// Checks if a model exists by its identification key.
    /// </summary>
    /// <param name="id">The identification key.</param>
    /// <returns>True if the model exists; otherwise, false.</returns>
    public virtual bool Exists(TKey id) => _readRepository.Exists(id);

    /// <summary>
    /// Checks if the specified model exists in the repository based on its <see cref="TModel.Id"/>.
    /// </summary>
    /// <param name="model">The model instance to verify.</param>
    /// <returns>True if a model with the same identification key exists; otherwise, false.</returns>
    public virtual bool Exists(TModel model) => _readRepository.Exists(model);

    /// <summary>
    /// Checks if a model exists based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <returns>True if any model satisfies the condition; otherwise, false.</returns>
    public virtual bool Exists(Expression<Func<TModel, bool>> predicate) => _readRepository.Exists(predicate);
    
    /// <summary>
    /// Checks if a model not exists by its identification key.
    /// </summary>
    /// <param name="id">The identification key.</param>
    /// <returns>True if the model not exists; otherwise, false.</returns>
    public virtual bool NotExists(TKey id) => _readRepository.NotExists(id);

    /// <summary>
    /// Checks if the specified model not exists in the repository based on its <see cref="TModel.Id"/>.
    /// </summary>
    /// <param name="model">The model instance to verify.</param>
    /// <returns>True if a model with the same identification key not exists; otherwise, false.</returns>
    public virtual bool NotExists(TModel model) => _readRepository.NotExists(model);

    /// <summary>
    /// Checks if a model not exists based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <returns>True if any model not satisfies the condition; otherwise, false.</returns>
    public virtual bool NotExists(Expression<Func<TModel, bool>> predicate) => _readRepository.NotExists(predicate);
    
    /// <summary>
    /// Asynchronously finds a model by its <see cref="TKey"/>.
    /// </summary>
    /// <param name="id">The identification key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    public Task<TModel?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        => this._readRepository.FindByIdAsync(id, cancellationToken);
    
    /// <summary>
    /// Asynchronously finds a collection of models by their <see cref="TKey"/> identifiers.
    /// </summary>
    /// <param name="ids">The collection of identification keys to search for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The model if found; otherwise, null.</returns>
    public Task<IEnumerable<TModel>> FindByIdAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        => this._readRepository.FindByIdAsync(ids, cancellationToken);
    
    /// <summary>
    /// Filters a sequence of models based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection of models that match the predicate. An empty list if nothing is found.</returns>
    public virtual Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
        => this._readRepository.FindAsync(predicate, cancellationToken);

    /// <summary>
    /// Filters a sequence of models with pagination support.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="filter">The pagination parameters to limit the result set and define the page offset.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection of models. An empty list if nothing is found.</returns>
    public virtual Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, PaginationFilter filter, CancellationToken cancellationToken = default)
        => this._readRepository.FindAsync(predicate, filter, cancellationToken);

    /// <summary>
    /// Asynchronously filters a sequence of models using a delegate to configure pagination.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="filter">A delegate to configure the pagination and search options.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of models. An empty list is returned if no models match the predicate.</returns>
    public virtual Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, Action<PaginationFilter> filter, CancellationToken cancellationToken = default)
        => this._readRepository.FindAsync(predicate, filter, cancellationToken);

    /// <summary>
    /// Provides an <see cref="IQueryable{TModel}"/> for advanced LINQ queries that are translated to MongoDB aggregation pipelines.
    /// </summary>
    /// <returns>An <see cref="IQueryable{TModel}"/> that allows further LINQ composition and deferred execution on the server side.</returns>
    public virtual IQueryable<TModel> Query() => this._readRepository.Query();
    
    /// <summary>
    /// Provides a filtered <see cref="IQueryable{TModel}"/> based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <returns>A filtered queryable for further composition.</returns>
    public virtual IQueryable<TModel> Query(Expression<Func<TModel, bool>> predicate) 
        => this._readRepository.Query(predicate);

    /// <summary>
    /// Counts the total number of models in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>The total number of models that match the criteria, or 0 if none are found.</returns>
    public virtual Task<long> CountAsync(CancellationToken cancellationToken = default) 
        => this._readRepository.CountAsync(cancellationToken);

    /// <summary>
    /// Counts models that satisfy a specific condition.
    /// </summary>
    /// <param name="predicate">A function to test each model for a condition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>The total number of models that match the criteria, or 0 if none are found.</returns>
    public virtual Task<long> CountAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
        => this._readRepository.CountAsync(predicate, cancellationToken);
    
    /// <summary>
    /// Retrieves all models. Caution: Not recommended for large datasets.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    public virtual Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => this._readRepository.GetAllAsync(cancellationToken);
    
    /// <summary>
    /// Retrieves all models using pagination.
    /// </summary>
    /// <param name="filter">The pagination parameters to limit the result set and define the page offset.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
    /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
    public virtual Task<IEnumerable<TModel>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
        => this._readRepository.GetAllAsync(filter, cancellationToken);
    
    public virtual void Dispose()
    {
        
    }
}