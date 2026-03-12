using System.Reflection;
using EntryManager.Shared.Data.MongoDB.Mappings;
using EntryManager.Shared.Domain.Abstractions.Objects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Shared.Data.MongoDB.Builders;

/// <summary>
/// Provides a fluent API for configuring the mapping between domain models and the database schema.
/// This class orchestrates the registration of <see cref="BsonClassMap"/> for entities and objects.
/// </summary>
public sealed class ModelBuilder
{
    /// <summary>
    /// Scans the specified assembly for all types that implement <see cref="IEntityTypeConfiguration{TModel}"/> 
    /// and applies their configurations automatically.
    /// </summary>
    /// <param name="assembly">The assembly to scan for configuration classes.</param>
    public void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var entityTypeConfigurationType = typeof(IEntityTypeConfiguration<>);

        assembly.GetTypes().Where(t => entityTypeConfigurationType.IsAssignableFrom(t));
    }

    /// <summary>
    /// Applies configuration that is defined in an <see cref="DefaultModelMap{TModel}"/> instance.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="configuration">The configuration to be applied.</param>
    /// <returns>
    /// The same <see cref="DefaultModelMap{TEntity}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    public ModelBuilder ApplyEntityMapping<TEntity>(IEntityTypeConfiguration<TEntity> configuration)
        where TEntity : IdentifiedObject<Guid>
    {
        var builder = Entity<TEntity>();

        configuration.Configure(builder);
        RegisterMapping(builder);
        
        return this;
    }

    /// <summary>
    /// Applies configuration defined in an <see cref="IEntityTypeConfiguration{TEntity}"/> instance.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <typeparam name="TKey">The type of the entity identification key.</typeparam>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The same <see cref="ModelBuilder"/> instance for method chaining.</returns>
    public ModelBuilder ApplyEntityMapping<TEntity, TKey>(IEntityTypeConfiguration<TEntity> configuration)
        where TEntity : IdentifiedObject<TKey>
    {
        var builder = Entity<TEntity, TKey>();

        configuration.Configure(builder);
        RegisterMapping(builder);

        return this;
    }

    /// <summary>
    /// Applies configuration that is defined in an <see cref="DefaultObjectMap{TObject}"/> instance.
    /// </summary>
    /// <typeparam name="TObject">The object type to be configured.</typeparam>
    /// <param name="configuration">The configuration to be applied.</param>
    /// <returns>
    /// The same <see cref="DefaultModelMap{TObject}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    public ModelBuilder ApplyObjectMapping<TObject>(IEntityTypeConfiguration<TObject> configuration)
        where TObject : class
    {
        var builder = Object<TObject>();

        configuration.Configure(builder);
        RegisterMapping(builder);

        return this;
    }

    /// <summary>
    /// Performs configuration of a given entity type in the model. If the entity type is not already part of 
    /// the model, it will be added to the model.
    /// This overload allows configuration of the entity type to be done in line in the method call rather than 
    /// being chained after a call to <see cref="DefaultModelMap{TEntity, TKey}"/>.
    /// This allows additional configuration at the model level to be chained after configuration for the 
    /// entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <typeparam name="TKey">The entity identification key.</typeparam>
    /// <returns>
    /// The same <see cref="DefaultModelMap{TEntity, TKey}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    private EntityTypeBuilder<TEntity> Entity<TEntity, TKey>() 
        where TEntity : IdentifiedObject<TKey>
    { 
        var builder = new EntityTypeBuilder<TEntity>();
        RegisterMapping(builder);

        return builder;
    }

    /// <summary>
    /// Performs configuration of a given entity type in the model. If the entity type is not already part of 
    /// the model, it will be added to the model.
    /// This overload allows configuration of the entity type to be done in line in the method call rather than 
    /// being chained after a call to <see cref="DefaultModelMap{TEntity, TKey}"/>.
    /// This allows additional configuration at the model level to be chained after configuration for the 
    /// entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <typeparam name="TKey">The entity identification key.</typeparam>
    /// <returns>
    /// The same <see cref="DefaultModelMap{TEntity, TKey}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    public ModelBuilder Entity<TEntity, TKey>(Action<EntityTypeBuilder<TEntity>> buildAction) 
        where TEntity : IdentifiedObject<TKey>
    {
        var builder = Entity<TEntity, TKey>();

        buildAction(builder);
        RegisterMapping(builder);

        return this;
    }

    /// <summary>
    /// Performs configuration of a given entity type in the model. If the entity type is not already part of 
    /// the model, it will be added to the model.
    /// This overload allows configuration of the entity type to be done in line in the method call rather than 
    /// being chained after a call to <see cref="DefaultModelMap{TEntity}"/>.
    /// This allows additional configuration at the model level to be chained after configuration for the 
    /// entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <returns>
    /// The same <see cref="DefaultModelMap{TEntity}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    private EntityTypeBuilder<TEntity> Entity<TEntity>() where TEntity : IdentifiedObject<Guid>
    {
        var builder = new EntityTypeBuilder<TEntity>();
        RegisterMapping(builder);

        return builder;
    }

    /// <summary>
    /// Performs configuration of a given entity type in the model. If the entity type is not already part of 
    /// the model, it will be added to the model.
    /// This overload allows configuration of the entity type to be done in line in the method call rather than 
    /// being chained after a call to <see cref="DefaultModelMap{TEntity}"/>.
    /// This allows additional configuration at the model level to be chained after configuration for the 
    /// entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <returns>
    /// The same <see cref="DefaultModelMap{TEntity}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    public ModelBuilder Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> buildAction) 
        where TEntity : IdentifiedObject<Guid>
    {
        var builder = Entity<TEntity>();
        
        buildAction(builder);
        RegisterMapping(builder);

        return this;
    }

    /// <summary>
    /// Performs configuration of a given object type in the model. If the object type is not already part of 
    /// the model, it will be added to the model.
    /// This overload allows configuration of the object type to be done in line in the method call rather than 
    /// being chained after a call to <see cref="DefaultObjectMap{TObject}"/>.
    /// This allows additional configuration at the model level to be chained after configuration for the 
    /// object type.
    /// </summary>
    /// <typeparam name="TObject">The object type to be configured.</typeparam>
    /// <returns>
    /// The same <see cref="DefaultObjectMap{Object}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    private EntityTypeBuilder<TObject> Object<TObject>() where TObject : class
    {
        var builder = new EntityTypeBuilder<TObject>();
        RegisterMapping(builder);

        return builder;
    }

    /// <summary>
    /// Performs configuration of a given object type in the model. If the object type is not already part of 
    /// the model, it will be added to the model.
    /// This overload allows configuration of the object type to be done in line in the method call rather than 
    /// being chained after a call to <see cref="DefaultObjectMap{TObject}"/>.
    /// This allows additional configuration at the model level to be chained after configuration for the 
    /// object type.
    /// </summary>
    /// <typeparam name="TObject">The object type to be configured.</typeparam>
    /// <returns>
    /// The same <see cref="DefaultObjectMap{Object}"/> instance so that additional configuration 
    /// calls can be chained.
    /// </returns>
    public ModelBuilder Object<TObject>(Action<EntityTypeBuilder<TObject>> buildAction) 
        where TObject : class
    {
        var builder = Object<TObject>();
        
        buildAction(builder);
        RegisterMapping(builder);

        return this;
    }

    /// <summary>
    /// Register an entity mapping if not already mapped.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="builder">
    /// The builder being used to construct the model for this context. Allow you to configure aspects of the 
    /// model that are specific to a given database.
    /// </param>
    private static void RegisterMapping<T>(EntityTypeBuilder<T> builder) where T : class
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            BsonClassMap.RegisterClassMap(builder);
    }
}