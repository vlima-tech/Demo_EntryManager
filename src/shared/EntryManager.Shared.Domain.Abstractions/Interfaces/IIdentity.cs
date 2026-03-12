namespace EntryManager.Shared.Domain.Abstractions;

public interface IIdentity<out TKey>
{
    /// <summary>
    /// Gets the unique identification key of the entity.
    /// </summary>
    TKey Id { get; }
}