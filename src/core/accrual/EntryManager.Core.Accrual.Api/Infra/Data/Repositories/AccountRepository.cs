using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;
using MongoDB.Driver;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class AccountRepository : BaseRepository<AccountModel, Guid>, IAccountRepository
{
    private readonly IAccountReadRepository _readRepository;
    
    public AccountRepository(AccrualContext context, IServiceBus serviceBus, IAccountReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.ACCOUNT)
    {
        _readRepository = readRepository;
    }

    public async Task<bool> UpdateBalanceAsync(Guid accountId, long amount, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<AccountModel>.Filter.Eq(x => x.Id, accountId);

            var update = Builders<AccountModel>.Update
                .Inc(x => x.Balance, amount);

            var options = new UpdateOptions { IsUpsert = true };

            var result = await this.Db.UpdateOneAsync(filter, update, options, cancellationToken);

            return result.IsAcknowledged;
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while updating account balance with ID: {accountId}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
            
            return false;
        }
    }
}