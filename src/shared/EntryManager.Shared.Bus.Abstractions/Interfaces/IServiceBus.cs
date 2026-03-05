using System.Runtime.CompilerServices;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Provides an abstraction over <a href="https://github.com/jbogard/MediatR">MediatR</a>, 
/// simplifying the development of modern enterprise applications that implement 
/// distributed systems, event-driven programming, microservices, and related patterns.
/// </summary>
public interface IServiceBus
{
    /// <summary>
    /// Publishes an <see cref="IEvent"/> to be handled by one or more handlers 
    /// within the current application when 
    /// <see cref="IEvent.ExecutionMode"/> is <see cref="ExecutionMode.Immediate"/>, 
    /// or enqueues it to the configured message broker when the mode is 
    /// <see cref="ExecutionMode.Enqueue"/>.
    /// </summary>
    /// <param name="event">The <see cref="IEvent"/> to publish.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    /// <returns>
    /// <see langword="true"/> if successfully executed or published; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    Task<bool> PublishAsync(IEvent @event, CancellationToken cancellationToken, [CallerMemberName] string sourceMethod = "", 
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "");

    /// <summary>
    /// Sends an <see cref="ICommand"/> to be handled by a single handler 
    /// within the current application when 
    /// <see cref="ICommand.ExecutionMode"/> is <see cref="ExecutionMode.Immediate"/>, 
    /// or enqueues it to the configured message broker when the mode is 
    /// <see cref="ExecutionMode.Enqueue"/>.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> to execute.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    /// <returns>
    /// <see langword="true"/> if successfully executed or published; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    Task<bool> SendAsync(ICommand command, CancellationToken cancellationToken, [CallerMemberName] string sourceMethod = "", 
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "");

    /// <summary>
    /// Sends a command that produces a response, handled by a single handler 
    /// within the current application.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The response type returned by <see cref="ICommand{TResponse}"/>.
    /// </typeparam>
    /// <param name="command">The <see cref="ICommand{TResponse}"/> to execute.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    /// <returns>
    /// A <typeparamref name="TResponse"/> instance if successfully executed; 
    /// otherwise, the default value.
    /// </returns>
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken, 
        [CallerMemberName] string sourceMethod = "", [CallerLineNumber] int sourceLineNumber = 0, 
        [CallerFilePath] string sourceFileName = "");
}