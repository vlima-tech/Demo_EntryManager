using EntryManager.Shared.Data.MongoDB.Builders;
using EntryManager.Shared.Data.MongoDB.Mappings;
using EntryManager.Shared.Data.MongoDB.Options;
using EntryManager.Shared.Domain.Abstractions;
using MongoDB.Driver;

namespace EntryManager.Shared.Data.MongoDB;

public class DbContext(DbContextOptions options) : IContextInitializer
{
    private DbContextOptions _options { get; set; } = options;
    private MongoClient _mongoClient { get; set; }
    protected IMongoDatabase Database { get; set; }
    private ModelBuilder _builder { get; set; } = new();
    private bool _initialized { get; set; } = false;

    private void Initialize()
    {
        OnModelCreating(_builder);

        // can be the last to call because MongoDB static configurations
        Connect();
        _initialized = true;
    }

    private void Connect()
    {
        _mongoClient = new MongoClient(_options.ConnectionString);
        Database = _mongoClient.GetDatabase(new MongoUrl(_options.ConnectionString).DatabaseName);
    }
    
    public IMongoCollection<TModel> Set<TModel>(string collection) 
        where TModel : class
    {
        if (!_initialized)
            Initialize();

        var collectionName = string.IsNullOrEmpty(collection) ? typeof(TModel).Name : collection;

        return Database.GetCollection<TModel>(collectionName);
    }

    public IMongoCollection<TModel> Set<TModel>() where TModel : class
    {
        if (!_initialized)
            Initialize();

        return Database.GetCollection<TModel>(typeof(TModel).Name);
    }

    /// <summary>
    /// Override this method to further configure the model that was discovered by convention
    /// from the entity types exposed in <see cref="Set{TModel}"/> or <seealso cref="Set{TModel}(string)"/>
    /// on your derived context. The resulting model may be cached and re-used for subsequent
    /// instances of your derived context.
    /// </summary>
    /// <param name="builder">
    /// The builder being used to construct the model for this context. Allow you to configure aspects of the 
    /// model that are specific to a given database.
    /// </param>
    protected virtual void OnModelCreating(ModelBuilder builder) 
        => builder.ApplyEntityMapping(new IdentifiedObjectMap());

    public virtual Task InitializeContextAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}