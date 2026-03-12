using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Mappings;

public class CategoryMapping : IEntityTypeConfiguration<CategoryModel>
{
    public void Configure(EntityTypeBuilder<CategoryModel> builder)
    {
        builder.AutoMap();
        
        builder.MapMember(c => c.EntryType)
            .SetSerializer(new EnumSerializer<EntryType>(BsonType.String));
        
        builder.UnmapMember(c => c.Group);
    }
}