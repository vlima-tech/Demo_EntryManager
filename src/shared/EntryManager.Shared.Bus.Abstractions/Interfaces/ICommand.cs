using MediatR;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// An execution order abstraction that together the necessary information to be executed.
/// </summary>
public interface ICommand : ICommand<bool>
{

}

/// <summary>
/// An execution order abstraction to be executed <see cref="Abstractions.ExecutionMode.Immediate"/> or
/// <see cref="ExecutionMode.Enqueue"/>.
/// </summary>
/// <typeparam name="TResponse">The result of command execution.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>, IContract
{
    /// <summary>
    /// Represents how the command is executed.
    /// In <see cref="Abstractions.ExecutionMode.Immediate"/> mode, it is executed locally.
    /// In <see cref="Abstractions.ExecutionMode.Enqueue"/> mode, it is sent to the configured
    /// message broker, enqueued, and executed later.
    /// </summary>
    ExecutionMode ExecutionMode { get; }

    /// <summary>
    /// Represents the assembly context of the inherited command.
    /// </summary>
    string ResourceType { get; }
    
    /// <summary>
    /// Configures the command to be handled within the current application.
    /// </summary>
    public void PrepareToExecute();

    /// <summary>
    /// Configures the command to be published to other services through a message broker.
    /// </summary>
    public void PrepareToSend();
}