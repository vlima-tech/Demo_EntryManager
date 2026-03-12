using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Mappings;

public class LedgerMapping : IEntityTypeConfiguration<LedgerModel>
{
    public void Configure(EntityTypeBuilder<LedgerModel> builder)
    {
        builder.AutoMap();
        
        builder.MapMember(l => l.Type)
            .SetSerializer(new EnumSerializer<ReferenceType>(BsonType.String));
    }
}