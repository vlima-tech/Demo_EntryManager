using EntryManager.Core.Transaction.Contracts.Objects;

namespace EntryManager.Core.Transaction.Contracts.Responses.GroupResponses;

public class ListGroupResponse(IEnumerable<GroupObject> groups) : List<GroupObject>(groups);