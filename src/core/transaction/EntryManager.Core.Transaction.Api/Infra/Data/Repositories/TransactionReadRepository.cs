using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Transaction.Api.Infra.Data.Repositories;

public class TransactionReadRepository : BaseReadRepository<TransactionModel, Guid>, ITransactionReadRepository, ITransactionQuery
{
    public TransactionReadRepository(TransactionContext context, IServiceBus serviceBus) 
        : base(context, serviceBus, Collections.TRANSACTION)
    { }

    public bool IsUniqueKey(string idempotencyKey) => this.NotExists(t => t.IdempotencyKey == idempotencyKey);
}