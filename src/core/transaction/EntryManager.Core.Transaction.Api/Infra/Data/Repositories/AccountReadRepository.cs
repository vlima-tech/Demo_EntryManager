using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Core.Transaction.Contracts.Enums;
using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Core.Transaction.Contracts.Responses.AccountResponses;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;
using MongoDB.Driver.Linq;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class AccountReadRepository : BaseReadRepository<AccountModel, Guid>, IAccountReadRepository, IAccountQuery
{
    public AccountReadRepository(TransactionContext context, IServiceBus serviceBus) 
        : base(context, serviceBus, Collections.ACCOUNT)
    { }

    public async Task<ListAccountResponse> ObtainsAllAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await base.GetAllAsync(cancellationToken);
        
        var result = accounts.Select(item => new AccountObject()
        {
            AccountId = item.Id,
            Name = item.Name,
            Balance = item.Balance,
            Status = (AccountStatus)item.Status
        });

        return new ListAccountResponse(result.ToList());
    }

    public async Task<AccountModel?> FindByNameAsync(string accountName, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = base.Query()
                .Where(a => a.Name.ToLowerInvariant().Equals(accountName.ToLowerInvariant()));

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            
            return result;
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while finding {nameof(AccountModel)} by name: {accountName}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }

        return default;
    }

    public bool Exists(string accountName)
    {
        var query = base.Query()
            .Where(a => a.Name.ToLowerInvariant().Equals(accountName.ToLowerInvariant()));

        return query.FirstOrDefault() is not null;
    }
    
    public bool NotExists(string accountName) => !this.Exists(accountName);

    async Task<FindAccountByIdResponse?> IAccountQuery.FindByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await base.FindByIdAsync(accountId, cancellationToken);

        if (account is null)
            return null;

        return new FindAccountByIdResponse
        {
            AccountId = account.Id,
            Name = account.Name,
            Balance = account.Balance,
            Status = (AccountStatus)account.Status
        };
    }
}