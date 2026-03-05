using FluentValidation.Results;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Represents an important fact that triggers workflows within the 
/// <a href="https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation">current application</a>
/// or across 
/// <a href="https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications">other services</a>.
/// </summary>
public abstract class Event : Contract, IEvent
{
    #region Properties
    
    /// <summary>
    /// Gets the execution mode of the event.  
    /// The default value is <see cref="Abstractions.ExecutionMode.Immediate"/>.
    /// Use <see cref="PrepareToSend"/> or <see cref="PrepareToExecute"/> to change it.
    /// </summary>
    public ExecutionMode ExecutionMode { get; private set; }

    /// <summary>
    /// Represents the assembly context of the inherited event.
    /// </summary>
    public string ResourceType { get; }
    
    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class.  
    /// By default, it executes immediately.  
    /// Call <see cref="PrepareToSend"/> to enqueue it for asynchronous execution.
    /// </summary>
    /// <param name="executionMode">The execution mode. Default is <see cref="Abstractions.ExecutionMode.Immediate"/>.</param>
    protected Event(ExecutionMode executionMode = ExecutionMode.Immediate)
    {
        this.ExecutionMode = executionMode;
        this.ResourceType = this.GetType().ToString();
    }

    /// <summary>
    /// Configures the event to be handled within the current application.
    /// </summary>
    public void PrepareToExecute() => this.ExecutionMode = ExecutionMode.Immediate;
    
    /// <summary>
    /// Configures the event to be published to other services through a message broker.
    /// </summary>
    public void PrepareToSend() => this.ExecutionMode = ExecutionMode.Enqueue;
}