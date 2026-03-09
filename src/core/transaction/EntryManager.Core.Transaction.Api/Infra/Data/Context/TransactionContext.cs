using System.Runtime.CompilerServices;
using EntryManager.Core.Transaction.Api.Infra.Data.Mappings;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using EntryManager.Shared.Data.MongoDB.Options;
using EntryManager.Shared.Domain.Abstractions.Objects;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Context;

public class TransactionContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyEntityMapping(new IdentifiedObjectMapping<Guid>());
        builder.ApplyEntityMapping(new AccountMapping());
        builder.ApplyEntityMapping(new GroupMapping());
        
        base.OnModelCreating(builder);
    }
}