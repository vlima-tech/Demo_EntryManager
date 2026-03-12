using EntryManager.Shared.Data.MongoDB.Builders;

namespace EntryManager.Shared.Data.MongoDB.Mappings;

public class DefaultObjectMap<TObject> : IEntityTypeConfiguration<TObject> where TObject : class
{
    public virtual void Configure(EntityTypeBuilder<TObject> builder)
    {
        builder.AutoMap();

        builder.SetDiscriminator(typeof(TObject).Name);
    }
}