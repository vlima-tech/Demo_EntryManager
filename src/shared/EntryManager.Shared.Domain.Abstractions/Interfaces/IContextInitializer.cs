namespace EntryManager.Shared.Domain.Abstractions;

public interface IContextInitializer
{
    /// <summary>
    /// Asynchronously seeds initial data into the database context.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous seeding operation.</returns>
    Task InitializeContextAsync(CancellationToken cancellationToken = default);
}