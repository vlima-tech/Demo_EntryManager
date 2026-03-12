using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class AccountRepository : BaseRepository<AccountModel, Guid>, IAccountRepository
{
    private readonly IAccountReadRepository _readRepository;
    
    public AccountRepository(TransactionContext context, IServiceBus serviceBus, IAccountReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.ACCOUNT)
    {
        _readRepository = readRepository;
    }

    public Task<AccountModel?> FindByNameAsync(string accountName, CancellationToken cancellationToken = default)
        => this._readRepository.FindByNameAsync(accountName, cancellationToken);

    public bool Exists(string accountName) => this._readRepository.Exists(accountName);
    
    public bool NotExists(string accountName) => this._readRepository.NotExists(accountName);
}