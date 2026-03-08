using Microsoft.Extensions.DependencyInjection;

namespace EntryManager.Shared.Data.MongoDB.Options;

/// <summary>
/// Represents a MongoDB setting
/// </summary>
public class DbContextOptions
{
    /// <summary>
    /// The MongoDB connection string
    /// </summary>
    public string ConnectionString { get; private set; }

    public ServiceLifetime ContextLifetime { get; set; } = ServiceLifetime.Scoped;
    
    public ServiceLifetime OptionsLifetime { get; set; } = ServiceLifetime.Scoped;
    
    /// <summary>
    /// Define the MongoDB database connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public void UseConnectionString(string connectionString) => ConnectionString = connectionString;
}