using EntryManager.Shared.Data.MongoDB.Builders;

namespace EntryManager.Shared.Data.MongoDB;

/// <summary>
/// Defines a contract for configuring the mapping and schema details of an entity 
/// outside the domain model, ensuring a decoupled architecture.
/// </summary>
/// <typeparam name="TModel">The entity type to be configured.</typeparam>
public interface IEntityTypeConfiguration<TModel> where TModel : class
{
    /// <summary>
    /// Configures the model of type <typeparamref name="TModel"/>.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the model type.</param>
    void Configure(EntityTypeBuilder<TModel> builder);
}