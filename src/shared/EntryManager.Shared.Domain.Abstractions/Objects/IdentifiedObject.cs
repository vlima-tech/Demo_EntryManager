namespace EntryManager.Shared.Domain.Abstractions.Objects;

/// <summary>
/// Represents an object identified by a unique key, with equality logic based on <see cref="Id"/>.
/// </summary>
/// <typeparam name="TKey">The type of the identification key.</typeparam>
public abstract class IdentifiedObject<TKey> : EquatableObject, IIdentity<TKey>
{
    /// <summary>
    /// The identification key of entity.
    /// </summary>
    public TKey Id { get; protected set; }

    #region Comparer Overrides

    /// <summary>
    /// Determines whether the specified object is equal to the current object by comparing reference or <see cref="Id"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the objects are considered equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not IdentifiedObject<TKey> compareTo) return false;
        if (ReferenceEquals(this, compareTo)) return true;

        // Garante que se o ID for o valor padrão (ex: Guid.Empty ou 0), 
        // ele não seja considerado igual a outro objeto com ID padrão.
        if (EqualityComparer<TKey>.Default.Equals(Id, default!) || EqualityComparer<TKey>.Default.Equals(compareTo.Id, default!))
            return false;

        return EqualityComparer<TKey>.Default.Equals(Id, compareTo.Id);
    }

    public override int GetHashCode()
        => (GetType().GetHashCode() ^ 93) + Id.GetHashCode();

    #endregion

    public override string ToString()
    { return GetType().Name + " [Id = " + Id + "]"; }
}