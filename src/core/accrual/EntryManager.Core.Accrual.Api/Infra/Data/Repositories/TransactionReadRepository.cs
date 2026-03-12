using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class TransactionReadRepository : BaseReadRepository<TransactionModel, Guid>, ITransactionReadRepository
{
    public TransactionReadRepository(AccrualContext context, IServiceBus serviceBus) 
        : base(context, serviceBus, Collections.TRANSACTION)
    {
    }
}