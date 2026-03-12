using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;
using MongoDB.Driver;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class LedgerRepository : BaseRepository<LedgerModel, long>, ILedgerRepository
{
    public LedgerRepository(AccrualContext context, IServiceBus serviceBus, ILedgerReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.LEDGER)
    {
    }

    public override async Task<bool> SaveAsync(LedgerModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<LedgerModel>.Filter.Eq(x => x.Id, model.Id);

            var update = Builders<LedgerModel>.Update
                .Inc(x => x.Value, model.Value)
                .SetOnInsert(x => x.Id, model.Id)
                .SetOnInsert(x => x.ReferenceId, model.ReferenceId)
                .SetOnInsert(x => x.EffectiveDay, model.EffectiveDay)
                .SetOnInsert(x => x.Type, model.Type)
                .SetOnInsert(x => x.CreatedAt, model.CreatedAt)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var options = new UpdateOptions { IsUpsert = true };

            var result = await this.Db.UpdateOneAsync(filter, update, options, cancellationToken);

            return result.IsAcknowledged;
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while updating a model of type {nameof(LedgerModel)} with ID: {model.Id}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
            
            return false;
        }
    }
}