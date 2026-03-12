using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class GroupReadRepository : BaseReadRepository<GroupModel, Guid>, IGroupReadRepository
{
    private readonly IAccountReadRepository _accountRepository;

    public GroupReadRepository(AccrualContext context, IServiceBus serviceBus, IServiceProvider provider)
        : base(context, serviceBus, Collections.GROUP)
        => this._accountRepository = provider.GetRequiredService<IAccountReadRepository>();

    public bool Exists(string groupName)
    {
        var query = base.Query()
            .Where(a => a.Name.ToLowerInvariant().Equals(groupName.ToLowerInvariant()));

        return query.FirstOrDefault() is not null;
    }

    public override async Task<GroupModel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var group = await base.FindByIdAsync(id, cancellationToken);

        if (group is null) return default;
        
        var account = await this._accountRepository.FindByIdAsync(group.AccountId, cancellationToken);
        
        return new GroupModel(group.Id, account.Id, group.Name, group.Description, group.Type);
    }
    
    public bool NotExists(string groupName) => !this.Exists(groupName);
}