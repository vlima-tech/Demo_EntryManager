using System.Linq.Expressions;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Domain.Abstractions;
using EntryManager.Shared.Domain.Abstractions.Objects;
using MongoDB.Driver;

namespace EntryManager.Shared.Data.MongoDB.Repositories;

public abstract class BaseReadRepository<TModel, TKey> : IBaseReadRepository<TModel, TKey>
        where TModel : IdentifiedObject<TKey>
    {
        protected IMongoCollection<TModel> Db { get; private set; }
        protected IServiceBus ServiceBus { get; private set; }

        #region Contructors

        public BaseReadRepository(DbContext context, IServiceBus serviceBus)
        {
            Db = context.Set<TModel>();
            ServiceBus = serviceBus;
        }

        public BaseReadRepository(DbContext context, IServiceBus serviceBus, string collection)
        {
            Db = context.Set<TModel>(collection);
            ServiceBus = serviceBus;
        }

        #endregion

        /// <summary>
        /// Checks if a model exists by its identification key.
        /// </summary>
        /// <param name="id">The identification key.</param>
        /// <returns>True if the model exists; otherwise, false.</returns>
        public virtual bool Exists(TKey id)
            => this.Db.Find(e => e.Id!.Equals(id)).Any();
        
        /// <summary>
        /// Checks if the specified model exists in the repository based on its <see cref="TModel.Id"/>.
        /// </summary>
        /// <param name="model">The model instance to verify.</param>
        /// <returns>True if a model with the same identification key exists; otherwise, false.</returns>
        public virtual bool Exists(TModel model)
            => Exists(model.Id);

        /// <summary>
        /// Checks if a model exists based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each model for a condition.</param>
        /// <returns>True if any model satisfies the condition; otherwise, false.</returns>
        public bool Exists(Expression<Func<TModel, bool>> predicate)
            => this.Db.Find(predicate).Any();
        
        /// <summary>
        /// Asynchronously finds a model by its <see cref="TKey"/>.
        /// </summary>
        /// <param name="id">The identification key.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The model if found; otherwise, null.</returns>
        public virtual async Task<TModel?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.Db.Find(m => m.Id!.Equals(id))
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch(Exception e)
            {
                var msg = $"An error occurred while finding a model of type {typeof(TModel).Name} with ID: {id}.";
                await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
            }

            return default;
        }
        
        /// <summary>
        /// Filters a sequence of models based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each model for a condition.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
        /// <returns>A collection of models that match the predicate. An empty list if nothing is found.</returns>
        public virtual async Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.Db.Find(predicate)
                    .ToListAsync(cancellationToken);
            }
            catch(Exception e)
            {
                var msg = $"An error occurred while finding models of type {typeof(TModel).Name} with the specified predicate.";
                await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
            }

            return [];
        }
        
        /// <summary>
        /// Filters a sequence of models with pagination support.
        /// </summary>
        /// <param name="predicate">A function to test each model for a condition.</param>
        /// <param name="filter">The pagination parameters to limit the result set and define the page offset.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
        /// <returns>A collection of models. An empty list if nothing is found.</returns>
        public virtual async Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, PaginationFilter filter, CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.Db.Find(predicate)
                    .Skip(filter.SkipLength)
                    .Limit(filter.PageSize)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception e)
            {
                var msg = $"An error occurred while finding paginated models of type {typeof(TModel).Name}.";
                await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
            }

            return [];
        }
        
        /// <summary>
        /// Asynchronously filters a sequence of models using a delegate to configure pagination.
        /// </summary>
        /// <param name="predicate">A function to test each model for a condition.</param>
        /// <param name="filter">A delegate to configure the pagination and search options.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A collection of models. An empty list is returned if no models match the predicate.</returns>
        public virtual Task<IEnumerable<TModel>> FindAsync(Expression<Func<TModel, bool>> predicate, Action<PaginationFilter> filter, CancellationToken cancellationToken = default)
        {
            var filterOptions = new PaginationFilter();
            filter.Invoke(filterOptions);

            return FindAsync(predicate, filterOptions, cancellationToken);
        }
        
        /// <summary>
        /// Provides an <see cref="IQueryable{TModel}"/> for advanced LINQ queries that are translated to MongoDB aggregation pipelines.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TModel}"/> that allows further LINQ composition and deferred execution on the server side.</returns>
        public IQueryable<TModel> Query() => this.Db.AsQueryable();
        
        /// <summary>
        /// Provides a filtered <see cref="IQueryable{TModel}"/> based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each model for a condition.</param>
        /// <returns>A filtered queryable for further composition.</returns>
        public virtual IQueryable<TModel> Query(Expression<Func<TModel, bool>> predicate) 
            => this.Db.AsQueryable().Where(predicate);
        
        /// <summary>
        /// Counts the total number of models in the repository.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The total number of models in the collection, or 0 if none are found.</returns>
        public virtual Task<long> CountAsync(CancellationToken cancellationToken = default)
            => this.Db.EstimatedDocumentCountAsync(cancellationToken: cancellationToken);

        /// <summary>
        /// Counts models that satisfy a specific condition.
        /// </summary>
        /// <param name="predicate">A function to test each model for a condition.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
        /// <returns>The total number of models that match the criteria, or 0 if none are found.</returns>
        public Task<long> CountAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
            => this.Db.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);

        /// <summary>
        /// Retrieves all models. Caution: Not recommended for large datasets.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
        /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
        public virtual async Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await Db.Find(Builders<TModel>.Filter.Empty)
                    .ToListAsync(cancellationToken);
            }
            catch(Exception e)
            {
                var msg = $"An error occurred on get all models of type {typeof(TModel).FullName}.";
                await ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
            }

            return [];
        }
        
        /// <summary>
        /// Retrieves all models using pagination.
        /// </summary>
        /// <param name="filter">The pagination parameters to limit the result set and define the page offset.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests</param>
        /// <returns>A collection containing all models. If no models exist, an empty list is returned.</returns>
        public virtual async Task<IEnumerable<TModel>> GetAllAsync(PaginationFilter filter, CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.Db.Find(Builders<TModel>.Filter.Empty)
                    .Skip(filter.SkipLength)
                    .Limit(filter.PageSize)
                    .ToListAsync(cancellationToken);
            }
            catch(Exception e)
            {
                var msg = $"An error occurred on get all models of type {typeof(TModel).FullName}.";
                await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
            }

            return [];
        }
        
        /// <summary>
        /// Retrieves all models using a delegate to configure pagination.
        /// </summary>
        /// <param name="filter">A delegate to configure the pagination and search options.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A collection of models. An empty list is returned if no models are found or if an error occurs.</returns>
        public virtual Task<IEnumerable<TModel>> GetAllAsync(Action<PaginationFilter> filter, CancellationToken cancellationToken = default)
        {
            var filterOptions = new PaginationFilter();

            filter?.Invoke(filterOptions);

            return this.GetAllAsync(filterOptions, cancellationToken);
        }
        
        public virtual void Dispose()
        {

        }
    }