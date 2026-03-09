using EntryManager.Core.Transaction.Contracts.Enums;
using EntryManager.Core.Transaction.Contracts.Objects;

namespace EntryManager.Core.Transaction.Contracts.Responses.CategoryResponses;

public class ListCategoryResponse(IEnumerable<ListCategoryResponse.CategoryObject> categories) : List<ListCategoryResponse.CategoryObject>(categories)
{
    public class CategoryObject
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public GroupType Type { get; set; }
    }
}