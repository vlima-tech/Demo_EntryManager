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
    
    public async Task<Rollup> LoadAsync(DateTime rollupDay, CancellationToken cancellationToken = default)
    {
        var datePart = rollupDay.ToString("yyyyMMdd");
        var rollup = new Rollup(rollupDay);
    
        var groupIds = await _redisDb.SetMembersAsync($"rollup:{datePart}:index:groups");

        foreach (var groupId in groupIds)
        {
            var groupKey = $"rollup:{datePart}:group:{groupId}";
        
            var groupData = await _redisDb.HashGetAllAsync(groupKey);
            
            if (groupData.Length == 0) continue;

            var groupDict = groupData.ToDictionary();
            
            var rollupGroup = new RollupGroup(
                Guid.Parse(groupDict[nameof(RollupGroup.GroupId)].ToString()),
                groupDict[nameof(RollupGroup.Name)],
                (long)groupDict[nameof(RollupGroup.Balance)]
            );

            var categoryIds = await _redisDb.SetMembersAsync($"{groupKey}:index:categories");
            var categoryList = new List<RollupCategory>();

            foreach (var categoryId in categoryIds)
            {
                var categoryKey = $"{groupKey}:category:{categoryId}";
            
                var catData = await _redisDb.HashGetAllAsync(categoryKey);
                if (catData.Length == 0) continue;

                var catDict = catData.ToDictionary();
                categoryList.Add(new RollupCategory(
                    Guid.Parse(catDict[nameof(RollupCategory.CategoryId)].ToString()),
                    catDict[nameof(RollupCategory.Title)],
                    (long)catDict[nameof(RollupCategory.Balance)]
                ));
                
                if(cancellationToken.IsCancellationRequested)
                    break;
            }

            rollup.Add(rollupGroup, categoryList);
            
            if(cancellationToken.IsCancellationRequested)
                break;
        }

        return rollup;
    }

    public async Task<bool> RemoveAsync(DateTime rollupDay, CancellationToken cancellationToken = default)
    {
        var datePart = rollupDay.ToString("yyyyMMdd");
        var groupIndexKey = $"rollup:{datePart}:index:groups";
    
        var groupIds = await _redisDb.SetMembersAsync(groupIndexKey);
    
        var batch = _redisDb.CreateBatch();
        var tasks = new List<Task>();

        foreach (var groupId in groupIds)
        {
            var groupKey = $"rollup:{datePart}:group:{groupId}";
            var categoryIndexKey = $"{groupKey}:index:categories";

            var categoryIds = await _redisDb.SetMembersAsync(categoryIndexKey);

            foreach (var categoryId in categoryIds)
                tasks.Add(batch.KeyDeleteAsync($"{groupKey}:category:{categoryId}"));
            
            tasks.Add(batch.KeyDeleteAsync(groupKey));
            tasks.Add(batch.KeyDeleteAsync(categoryIndexKey));
        }

        tasks.Add(batch.KeyDeleteAsync(groupIndexKey));

        batch.Execute();
        await Task.WhenAll(tasks);
        
        return true;
    }
}