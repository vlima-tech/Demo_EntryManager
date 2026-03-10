using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Transaction.Api.Domain.Models;

public class CategoryModel : BaseModel
{
    public Guid GroupId { get; private set; }
    
    public string Name { get; private set; }
    
    public EntryType EntryType { get; private set; }

    public GroupModel Group { get; private set; }

    public CategoryModel()
    { }

    public CategoryModel(string name, GroupModel group)
    {
        this.Name = name;
        this.GroupId = group.Id;
        this.EntryType = group.Type;
        this.Group = group;
    }
    
    public CategoryModel(Guid id, string name, GroupModel group) : this(name, group) => this.Id = id;

    public static CategoryModel Create(string name, GroupModel group) => new(name, group);
    
    public static CategoryModel Create(Guid id, string name, GroupModel group) => new(id, name, group);
    
    public void ChangeName(string categoryName)
    {
        if (!string.IsNullOrWhiteSpace(categoryName.Trim()))
            this.Name = categoryName;
    }
}