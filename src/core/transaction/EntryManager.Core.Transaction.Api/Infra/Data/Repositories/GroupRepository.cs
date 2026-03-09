using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class GroupRepository : BaseRepository<GroupModel, Guid>, IGroupRepository
{
    private readonly IGroupReadRepository _readRepository;
    
    public GroupRepository(TransactionContext context, IServiceBus serviceBus, IGroupReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.GROUP)
    {
        _readRepository = readRepository;
    }

    public bool Exists(string groupName)
        => this._readRepository.Exists(groupName);

    public bool NotExists(string groupName)
        => this._readRepository.NotExists(groupName);
}