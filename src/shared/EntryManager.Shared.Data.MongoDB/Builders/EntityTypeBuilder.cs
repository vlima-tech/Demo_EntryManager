using MongoDB.Bson.Serialization;

namespace EntryManager.Shared.Data.MongoDB.Builders;

/// <summary>
/// A builder used to define schemas, indexes, and mapping rules for <typeparamref name="TModel"/> 
/// within the persistence layer.
/// </summary>
/// <typeparam name="TModel">The model type being configured.</typeparam>
public class EntityTypeBuilder<TModel> : BsonClassMap<TModel> where TModel : class
{ }