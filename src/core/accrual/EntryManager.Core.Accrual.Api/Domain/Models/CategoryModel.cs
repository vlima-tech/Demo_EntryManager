using EntryManager.Core.Accrual.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Accrual.Api.Domain.Models;

public class CategoryModel : BaseModel
{
    public Guid GroupId { get; private set; }
    
    public string Name { get; private set; }
    
    public EntryType EntryType { get; private set; }

    public GroupModel Group { get; private set; }

    protected CategoryModel()
    { }

    public CategoryModel(Guid categoryId, string name, GroupModel group)
    {
        this.Id = categoryId;
        this.GroupId = group.Id;
        this.Name = name;
        this.EntryType = group.Type;
        this.Group = group;
    }
    
    public void ChangeName(string categoryName)
    {
        if (!string.IsNullOrWhiteSpace(categoryName.Trim()))
            this.Name = categoryName;
    }
}