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
    
    public override async Task<GroupModel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var group = await base.FindByIdAsync(id, cancellationToken);

        if (group is null) return default;
        
        var account = await this._accountRepository.FindByIdAsync(group.AccountId, cancellationToken);
        
        return new GroupModel(group.Id, group.Name, group.Description, group.Type, account);
    }
    
    
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

    async Task<ListGroupResponse> IGroupQuery.ObtainsAllAsync(CancellationToken cancellationToken)
    {
        var result = await this.GetAllAsync(cancellationToken);

        var groupObjects = result.Select(group => new GroupObject
        {
            GroupId = group.Id,
            Name = group.Name,
            Type = (Contracts.Enums.EntryType) group.Type,
            Account = new AccountObject
            {
                AccountId = group.AccountId,
                Name = group.Account.Name,
                Status = (Contracts.Enums.AccountStatus) group.Account.Status
            }
        });
        
        return new ListGroupResponse(groupObjects);
    }

    async Task<FindGroupByIdResponse?> IGroupQuery.ObtainsByIdAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var group = await this.FindByIdAsync(groupId, cancellationToken);
        
        if (group is null) return default;
        
        return new FindGroupByIdResponse
        {
            GroupId = group.Id,
            Name = group.Name,
            Type = (Contracts.Enums.EntryType) group.Type,
            Account = new AccountObject
            {
                AccountId = group.AccountId,
                Name = group.Account.Name,
                Status = (Contracts.Enums.AccountStatus) group.Account.Status
            }
        };
    }
}