using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Mappings;

public class TransactionMapping : IEntityTypeConfiguration<TransactionModel>
{
    public void Configure(EntityTypeBuilder<TransactionModel> builder)
    {
        builder.AutoMap();
        
        builder.MapMember(trn => trn.Status)
            .SetSerializer(new EnumSerializer<TransactionStatus>(BsonType.String));
    }
}