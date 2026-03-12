using EntryManager.Shared.Data.MongoDB.Builders;
using EntryManager.Shared.Domain.Abstractions.Objects;

namespace EntryManager.Shared.Data.MongoDB.Mappings;

public class DefaultModelMap<TModel> : DefaultModelMap<TModel, Guid>
    where TModel : IdentifiedObject<Guid>
{
    public override void Configure(EntityTypeBuilder<TModel> builder) => base.Configure(builder);
}

public class DefaultModelMap<TModel, TKey> : IEntityTypeConfiguration<TModel>
    where TModel : IdentifiedObject<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TModel> builder)
    {
        builder.AutoMap();
            
        builder.SetDiscriminator(typeof(TModel).Name);
    }
}