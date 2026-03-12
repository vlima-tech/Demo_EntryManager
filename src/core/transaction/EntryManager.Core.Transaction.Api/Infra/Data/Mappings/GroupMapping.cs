using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Mappings;

public class GroupMapping : IEntityTypeConfiguration<GroupModel>
{
    public void Configure(EntityTypeBuilder<GroupModel> builder)
    {
        builder.AutoMap();
        
        builder.MapCreator(group => new GroupModel());

        builder.MapMember(group => group.AccountId)
            .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        
        builder.MapMember(acc => acc.Type)
            .SetSerializer(new EnumSerializer<EntryType>(BsonType.String));

        builder.UnmapMember(group => group.Account);
        
        builder.SetIsRootClass(true);
    }
}