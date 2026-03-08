using EntryManager.Shared.Data.MongoDB.Builders;
using EntryManager.Shared.Domain.Abstractions.Objects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Shared.Data.MongoDB.Mappings;

public class IdentifiedObjectMap : IEntityTypeConfiguration<IdentifiedObject<Guid>>
{
    public virtual void Configure(EntityTypeBuilder<IdentifiedObject<Guid>> builder)
    {
        builder.AutoMap();

        builder.MapProperty(io => io.Id)
            .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
            .SetIdGenerator(CombGuidGenerator.Instance);
    }
}

public class IdentifiedObjectMap<TKey> : IEntityTypeConfiguration<IdentifiedObject<TKey>>
{
    public virtual void Configure(EntityTypeBuilder<IdentifiedObject<TKey>> builder)
    {
        builder.AutoMap();
    }
}