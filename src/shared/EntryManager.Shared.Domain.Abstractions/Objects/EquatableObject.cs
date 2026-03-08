using System.Runtime.CompilerServices;

namespace EntryManager.Shared.Domain.Abstractions.Objects;

/// <summary>
/// A base object with basic equality features.
/// </summary>
public abstract class EquatableObject
{
    #region Comparer Overrides

    public override bool Equals(object? obj)
    {
        var compareTo = obj as EquatableObject;

        return !ReferenceEquals(null, compareTo) && ReferenceEquals(this, compareTo);
    }

    public static bool operator ==(EquatableObject a, EquatableObject b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(EquatableObject a, EquatableObject b) => !(a == b); 

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    #endregion
}