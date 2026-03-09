using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using EntryManager.Shared.Domain.Abstractions.Objects;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Mappings;

public class IdentifiedObjectMapping<TId> : IEntityTypeConfiguration<IdentifiedObject<TId>>
{
    public void Configure(EntityTypeBuilder<IdentifiedObject<TId>> builder)
    {
        builder.AutoMap();
        
        builder.MapIdMember(obj => obj.Id);
        
        builder.SetDiscriminator(typeof(IdentifiedObject<TId>).Name);
    }
}