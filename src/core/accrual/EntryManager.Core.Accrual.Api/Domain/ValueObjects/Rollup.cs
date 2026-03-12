using EntryManager.Core.Accrual.Api.Domain.Models;

namespace EntryManager.Core.Accrual.Api.Domain.ValueObjects;

public class Rollup(DateTime rollupDay) : Dictionary<RollupGroup, List<RollupCategory>>
{
    public string Id => this.ToString();
    
    public DateTime RollupDay { get; } = rollupDay;
    
    public bool ContainsGroup(Guid groupId) => this.Any(r => r.Key.GroupId == groupId);
    
    public List<RollupCategory> GetCategories(Guid groupId) 
        => this.FirstOrDefault(r => r.Key.GroupId == groupId).Value ?? [];

    public bool ContainsKey(Guid categoryId) => this.Any(r => r.Key.GroupId == categoryId);
    
    public List<RollupCategory?> this[Guid categoryId] 
        => this.FirstOrDefault(r => r.Key.GroupId == categoryId).Value;
    
    public void Add(CategoryModel category, IEnumerable<TransactionModel> transactions)
    {
        long totalAmount = transactions.Sum(t => t.Value);

        var entry = this.FirstOrDefault(x => x.Key.GroupId == category.GroupId);

        RollupGroup currentGroup;
        List<RollupCategory> categories;

        if (entry.Key == null)
        {
            currentGroup = new RollupGroup(category.Group.Id, category.Group.Name, 0);
            categories = [];
        }
        else
        {
            currentGroup = entry.Key;
            categories = entry.Value;
        }

        var existingCategory = categories.FirstOrDefault(c => c.CategoryId == category.Id);
    
        if (existingCategory == null)
        {
            existingCategory = new RollupCategory(category.Id, category.Name, 0);
            categories.Add(existingCategory);
        }

        var updatedCategory = existingCategory with { Balance = existingCategory.Balance + totalAmount };
        var updatedGroup = currentGroup with { Balance = currentGroup.Balance + totalAmount };

        var catIndex = categories.IndexOf(existingCategory);
        categories[catIndex] = updatedCategory;

        if (entry.Key != null)
            this.Remove(entry.Key);
    
        this.Add(updatedGroup, categories);
    }
    
    public override string ToString()
        => $"rollup:{this.RollupDay:yyyyMMdd}";
}