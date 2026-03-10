using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class TransactionRepository : BaseRepository<TransactionModel, Guid>, ITransactionRepository, ITransactionReadRepository
{
    private readonly ITransactionReadRepository _readRepository;
    
    public TransactionRepository(TransactionContext context, IServiceBus serviceBus, ITransactionReadRepository readRepository) 
        : base(context, serviceBus, readRepository, Collections.TRANSACTION)
        => this._readRepository = readRepository;

    public bool IsUniqueKey(string idempotencyKey)
        => this._readRepository.IsUniqueKey(idempotencyKey);
}