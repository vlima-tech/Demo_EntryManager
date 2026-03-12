using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

public class CheckCategoryStep(IServiceProvider provider, IRegisterTransactionProcessor processor) : IRegisterTransactionProcessor
{
    private ICategoryRepository _categoryRepository = provider.GetRequiredService<ICategoryRepository>();
    private INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task ProcessAsync(RegisterTransactionTransfer transfer, CancellationToken cancellationToken)
    {
        var categoryId = transfer.Event.Transaction.CategoryId;

        if (this._categoryRepository.Exists(categoryId))
        {
            await processor.ProcessAsync(transfer, cancellationToken);
            return;
        }
        
        var categoryObj = transfer.Event.Transaction.Category;
        var groupObj = categoryObj.Group;
        var groupEntryType = (Domain.Enums.EntryType)categoryObj.Group.Type;
        
        var group = new GroupModel(groupObj.GroupId, groupObj.Account.AccountId, groupObj.Name, groupObj.Description, groupEntryType);
        var categoryModel = new CategoryModel(categoryId, categoryObj.Title, group);
        
        await this._categoryRepository.CreateAsync(categoryModel, cancellationToken);
        
        if(this._store.HasNotifications())
            return;
        
        await processor.ProcessAsync(transfer, cancellationToken);
    }
    
    public void Dispose()
    {
        this._categoryRepository?.Dispose();
        this._categoryRepository = null;
    }
}