using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Mappings;

public class AccountMapping : IEntityTypeConfiguration<AccountModel>
{
    public void Configure(EntityTypeBuilder<AccountModel> builder)
    {
        builder.AutoMap();
        
        builder.MapCreator(acc => new AccountModel(acc.Id, acc.Name, acc.Balance, acc.Status));
        
        builder.MapMember(acc => acc.Status)
            .SetSerializer(new EnumSerializer<AccountStatus>(BsonType.String));
        
        builder.SetIgnoreExtraElements(true);
    }
}