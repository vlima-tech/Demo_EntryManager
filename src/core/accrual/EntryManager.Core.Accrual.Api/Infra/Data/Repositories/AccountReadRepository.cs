using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Infra.Data.Context;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Data.MongoDB.Repositories;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class AccountReadRepository : BaseReadRepository<AccountModel, Guid>, IAccountReadRepository
{
    public AccountReadRepository(AccrualContext context, IServiceBus serviceBus) 
        : base(context, serviceBus, Collections.ACCOUNT) { }
}