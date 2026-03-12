using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Mappings;

public class CategoryMapping : IEntityTypeConfiguration<CategoryModel>
{
    public void Configure(EntityTypeBuilder<CategoryModel> builder)
    {
        builder.AutoMap();
        
        builder.UnmapMember(x => x.Group);
    }
}