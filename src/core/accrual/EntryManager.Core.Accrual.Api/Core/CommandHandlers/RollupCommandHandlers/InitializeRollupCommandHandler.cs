using EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.Models;
using EntryManager.Core.Accrual.Api.Domain.ValueObjects;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Accrual.Api.Core.CommandHandlers.RollupCommandHandlers;

public class InitializeRollupCommandHandler(IServiceProvider provider) : ICommandHandler<InitializeRollupCommand>
{
    private IGroupReadRepository _groupRepository = provider.GetRequiredService<IGroupReadRepository>();
    private ICategoryReadRepository _categoryRepository = provider.GetRequiredService<ICategoryReadRepository>();
    private ITransactionReadRepository _transactionRepository = provider.GetRequiredService<ITransactionReadRepository>();
    private readonly IRollupRepository _rollupRepository = provider.GetRequiredService<IRollupRepository>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public async Task<bool> Handle(InitializeRollupCommand command, CancellationToken cancellationToken)
    {
        var rollupExists = this._rollupRepository.Exists(command.RollupDay);
        
        if(rollupExists)
            return true;
        
        var rollup = new Rollup(command.RollupDay);
        
        var categories = await this.ObtainsCategoriesAsync(cancellationToken);

        foreach (var category in categories)
        {
            var transactions = await this._transactionRepository.FindAsync(t => t.CategoryId == category.Id, cancellationToken);
            rollup.Add(category, transactions);
        }

        await this._rollupRepository.InitializeAsync(rollup, cancellationToken);

        return !this._store.HasNotifications();
    }

    private async Task<IEnumerable<CategoryModel>> ObtainsCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = new List<CategoryModel>();
        var tasks = new List<Task>();
        
        var groups = await this._groupRepository.GetAllAsync(cancellationToken);

        foreach (var group in groups)
        {
            var findTask = this._categoryRepository.FindAsync(c => c.GroupId == group.Id, cancellationToken)
                .ContinueWith(t => categories.AddRange(t.Result), cancellationToken);
            
            tasks.Add(findTask);
        }
        
        Task.WaitAll(tasks.ToArray(), cancellationToken);
        
        return categories;
    }

    public void Dispose()
    {
        this._groupRepository?.Dispose();
        this._groupRepository = null;
        
        this._categoryRepository?.Dispose();
        this._categoryRepository = null;
        
        this._transactionRepository?.Dispose();
        this._transactionRepository = null;
    }
}