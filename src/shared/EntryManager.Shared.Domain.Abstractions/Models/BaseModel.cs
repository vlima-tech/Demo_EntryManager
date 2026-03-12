using EntryManager.Shared.Domain.Abstractions.Objects;

namespace EntryManager.Shared.Domain.Abstractions.Models;

/// <summary>
/// A base model with Guid as the default identification key.
/// </summary>
public class BaseModel : BaseModel<Guid>
{
    protected BaseModel(bool generateId = true)
    {
        if (generateId) Id = Guid.NewGuid();
    }
}

/// <summary>
/// A base model with equality and identification features.
/// </summary>
public abstract class BaseModel<TKey> : IdentifiedObject<TKey> { }