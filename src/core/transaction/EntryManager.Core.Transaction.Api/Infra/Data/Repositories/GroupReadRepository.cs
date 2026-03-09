using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Core.Transaction.Contracts.Responses.GroupResponses;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;
using MongoDB.Driver.Linq;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class GroupReadRepository : BaseReadRepository<GroupModel, Guid>, IGroupReadRepository, IGroupQuery
{
    private readonly IAccountReadRepository _accountRepository;

    public GroupReadRepository(TransactionContext context, IServiceBus serviceBus, IServiceProvider provider)
        : base(context, serviceBus, Collections.GROUP)
        => this._accountRepository = provider.GetRequiredService<IAccountReadRepository>();

    public bool Exists(string groupName)
    {
        var query = base.Query()
            .Where(a => a.Name.ToLowerInvariant().Equals(groupName.ToLowerInvariant()));

        return query.FirstOrDefault() is not null;
    }

    public bool NotExists(string groupName) => !this.Exists(groupName);
    
    public override async Task<IEnumerable<GroupModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var groups = await base.GetAllAsync(cancellationToken);

        var accountIds = groups.Select(g => g.AccountId)
            .Distinct();
        
        var accounts = (await this._accountRepository.FindByIdAsync(accountIds, cancellationToken))
            .ToDictionary(a => a.Id);
        
        var finalGroups = groups.Select(group => accounts.TryGetValue(group.AccountId, out var acc) 
            ? new GroupModel(group.Id, group.Name, group.Description, group.Type, acc) 
            : group);

        return finalGroups.ToList();
    }

    async Task<ListGroupResponse> IGroupQuery.GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await this.GetAllAsync(cancellationToken);

        var groupObjects = result.Select(group => new GroupObject
        {
            Id = group.Id,
            Name = group.Name,
            Type = (Contracts.Enums.GroupType) group.Type,
            Account = group.Account.Name
        });
        
        return new ListGroupResponse(groupObjects);
    }

    public async Task<FindGroupByNameResponse?> FindByNameAsync(string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = base.Query()
                .Where(a => a.Name.ToLowerInvariant().Equals(groupName.ToLowerInvariant()));
            
            var group = await query.FirstOrDefaultAsync(cancellationToken);
            var account = await this._accountRepository.FindByIdAsync(group.AccountId, cancellationToken);
            
            return new FindGroupByNameResponse
            {
                Id = group.Id,
                Name = group.Name,
                Type = (Contracts.Enums.GroupType) group.Type,
                Account = account.Name
            };
        }
        catch (Exception e)
        {
            var msg = $"An error occurred while finding {nameof(GroupModel)} by name: {groupName}.";
            await this.ServiceBus.PublishAsync(new SystemError(msg, e), cancellationToken);
        }

        return default;
    }
}