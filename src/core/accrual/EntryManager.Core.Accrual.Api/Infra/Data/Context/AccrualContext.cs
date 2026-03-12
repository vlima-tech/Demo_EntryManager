using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Mappings;
using EntryManager.Core.Transaction.Api.Infra.Data.Mappings;
using EntryManager.Shared.Data.MongoDB;
using EntryManager.Shared.Data.MongoDB.Builders;
using EntryManager.Shared.Data.MongoDB.Options;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Context;

public class AccrualContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyEntityMapping(new IdentifiedObjectMapping<Guid>());
        builder.ApplyEntityMapping(new AccountMapping());
        builder.ApplyEntityMapping(new GroupMapping());
        builder.ApplyEntityMapping(new CategoryMapping());
        builder.ApplyEntityMapping(new TransactionMapping());
        builder.ApplyEntityMapping<LedgerModel, long>(new LedgerMapping());
        
        base.OnModelCreating(builder);
    }
}