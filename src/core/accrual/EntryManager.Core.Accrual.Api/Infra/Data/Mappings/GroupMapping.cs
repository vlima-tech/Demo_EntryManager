using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Mappings;

public class GroupMapping : IEntityTypeConfiguration<GroupModel>
{
    public void Configure(EntityTypeBuilder<GroupModel> builder)
    {
        builder.AutoMap();
        
        builder.MapMember(acc => acc.Type)
            .SetSerializer(new EnumSerializer<EntryType>(BsonType.String));
    }
}