using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Domain.ValueObjects;
using StackExchange.Redis;

namespace EntryManager.Core.Accrual.Api.Infra.Data.Repositories;

public class RollupRepository(IServiceProvider provider) : IRollupRepository
{
    private readonly IDatabase _redisDb = provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
    
    public bool Exists(DateTime rollupDay)
    {
        var datePart = rollupDay.ToString("yyyyMMdd");
    
        var indexKey = $"rollup:{datePart}:index:groups";

        return _redisDb.KeyExists(indexKey);
    }
    
    public Task<bool> InitializeAsync(Rollup rollup, CancellationToken cancellationToken = default)
    {
        var rollupDay = rollup.RollupDay.ToString("yyyyMMdd");
        var batch = this._redisDb.CreateBatch();

        List<Task> tasks = [];
        
        foreach (var rollupGroup in rollup)
        {
            var groupKey = $"rollup:{rollupDay}:group:{rollupGroup.Key.GroupId}";
            
            tasks.Add(batch.HashSetAsync(groupKey, nameof(RollupGroup.Balance), rollupGroup.Key.Balance, When.NotExists));
            tasks.Add(batch.HashSetAsync(groupKey, nameof(RollupGroup.Name), rollupGroup.Key.Name, When.NotExists));
            tasks.Add(batch.HashSetAsync(groupKey, nameof(RollupGroup.GroupId), rollupGroup.Key.GroupId.ToString(), When.NotExists));

            tasks.Add(batch.SetAddAsync($"rollup:{rollupDay}:index:groups", rollupGroup.Key.GroupId.ToString()));

            foreach (var rollupCategory in rollup[rollupGroup.Key])
            {
                var categoryKey = $"{groupKey}:category:{rollupCategory.CategoryId}";

                tasks.Add(batch.HashSetAsync(categoryKey, nameof(RollupCategory.Balance), rollupCategory.Balance, When.NotExists));
                tasks.Add(batch.HashSetAsync(categoryKey, nameof(RollupCategory.Title), rollupCategory.Title, When.NotExists));
                tasks.Add(batch.HashSetAsync(categoryKey, nameof(RollupCategory.CategoryId), rollupCategory.CategoryId.ToString(), When.NotExists));

                tasks.Add(batch.SetAddAsync($"{groupKey}:index:categories", rollupCategory.CategoryId.ToString()));
                
                if(cancellationToken.IsCancellationRequested)
                    break;
            }
            
            if(cancellationToken.IsCancellationRequested)
                break;
        }
        
        batch.Execute();
        Task.WaitAll(tasks.ToArray(), cancellationToken);
        
        return Task.FromResult(true);
    }
}