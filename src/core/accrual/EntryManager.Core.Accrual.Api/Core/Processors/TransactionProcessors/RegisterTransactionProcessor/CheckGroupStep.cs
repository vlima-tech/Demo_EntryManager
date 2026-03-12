using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

public class CheckGroupStep(IServiceProvider provider, IRegisterTransactionProcessor processor) : IRegisterTransactionProcessor
{
    private IGroupRepository _groupRepository = provider.GetRequiredService<IGroupRepository>();
    private INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task ProcessAsync(RegisterTransactionTransfer transfer, CancellationToken cancellationToken)
    {
        var groupId = transfer.Event.Transaction.GroupId;

        if (this._groupRepository.Exists(groupId))
        {
            await processor.ProcessAsync(transfer, cancellationToken);
            return;
        }
        
        var groupObj = transfer.Event.Transaction.Category.Group;
        var accountId = groupObj.Account.AccountId;
        var groupObjType = (Domain.Enums.EntryType)groupObj.Type;
        
        var groupModel = new GroupModel(groupId, accountId, groupObj.Name, groupObj.Description, groupObjType);

        await this._groupRepository.CreateAsync(groupModel, cancellationToken);
        
        if(this._store.HasNotifications())
            return;
        
        await processor.ProcessAsync(transfer, cancellationToken);
    }
    
    public void Dispose()
    {
        this._groupRepository?.Dispose();
        this._groupRepository = null;
    }
}