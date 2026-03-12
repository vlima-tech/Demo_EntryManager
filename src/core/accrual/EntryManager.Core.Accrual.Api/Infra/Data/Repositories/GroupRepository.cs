using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class GroupRepository : BaseRepository<GroupModel, Guid>, IGroupRepository
{
    private readonly IGroupReadRepository _readRepository;
    
    public GroupRepository(AccrualContext context, IServiceBus serviceBus, IGroupReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.GROUP)
    {
        _readRepository = readRepository;
    }

    public bool Exists(string groupName)
        => this._readRepository.Exists(groupName);

    public bool NotExists(string groupName)
        => this._readRepository.NotExists(groupName);
}