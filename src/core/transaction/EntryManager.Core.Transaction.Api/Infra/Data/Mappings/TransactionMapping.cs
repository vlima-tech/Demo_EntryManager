using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Mappings;

public class TransactionMapping : IEntityTypeConfiguration<TransactionModel>
{
    public void Configure(EntityTypeBuilder<TransactionModel> builder)
    {
        builder.AutoMap();
        
        builder.UnmapMember(t => t.Category);
    }
}