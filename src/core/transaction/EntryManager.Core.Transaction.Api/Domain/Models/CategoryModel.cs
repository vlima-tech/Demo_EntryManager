using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Shared.Domain.Abstractions.Models;

namespace EntryManager.Core.Transaction.Api.Domain.Models;

public class CategoryModel : BaseModel
{
    public Guid GroupId { get; private set; }
    
    public string Title { get; private set; }
    
    public EntryType EntryType { get; private set; }

    public GroupModel Group { get; private set; }

    public CategoryModel()
    { }

    public CategoryModel(string title, GroupModel group)
    {
        this.Title = title;
        this.GroupId = group.Id;
        this.EntryType = group.Type;
        this.Group = group;
    }
    
    public CategoryModel(Guid id, string title, GroupModel group) : this(title, group) => this.Id = id;

    public static CategoryModel Create(string title, GroupModel group) => new(title, group);
    
    public static CategoryModel Create(Guid id, string title, GroupModel group) => new(id, title, group);
    
    public void ChangeTitle(string title)
    {
        if (!string.IsNullOrWhiteSpace(title.Trim()))
            this.Title = title;
    }
}