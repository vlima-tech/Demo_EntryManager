using MediatR;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// An abstraction of important fact occurred on application that can be executed <see cref="Abstractions.ExecutionMode.Immediate"/> or not by one or more
/// <a href="https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications">consumers</a>.
/// </summary>
public interface IEvent : INotification, IContract
{
    /// <summary>
    /// Represents how the event is executed.
    /// In <see cref="Abstractions.ExecutionMode.Immediate"/> mode, it is executed locally.
    /// In <see cref="Abstractions.ExecutionMode.Enqueue"/> mode, it is sent to the configured
    /// message broker, enqueued, and executed later.
    /// </summary>
    public ExecutionMode ExecutionMode { get; }
    
    /// <summary>
    /// Represents the assembly context of the inherited command.
    /// </summary>
    string ResourceType { get; }

    /// <summary>
    /// Configures the event to be handled within the current application.
    /// </summary>
    public void PrepareToExecute();

    /// <summary>
    /// Configures the event to be published to other services through a message broker.
    /// </summary>
    public void PrepareToSend();
}