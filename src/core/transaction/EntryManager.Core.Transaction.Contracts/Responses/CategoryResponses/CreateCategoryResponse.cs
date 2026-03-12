using EntryManager.Core.Transaction.Contracts.Objects;

namespace EntryManager.Core.Transaction.Contracts.Responses.CategoryResponses;

public class CreateCategoryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public GroupObject Group { get; set; }
}