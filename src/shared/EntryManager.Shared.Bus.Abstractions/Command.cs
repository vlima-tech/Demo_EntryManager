using FluentValidation;
using FluentValidation.Results;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Represents a system feature or action that can be executed within the current application 
/// <a href="https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-implementation-web-api">immediately</a>
/// or triggered asynchronously by 
/// <a href="https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/asynchronous-message-based-communication">another service</a>.
/// </summary>
public abstract class Command : Command<bool>, ICommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Command"/> class that executes immediately by default.  
    /// Set <see cref="Command{TResponse}.ExecutionMode"/> to <see cref="ExecutionMode.Enqueue"/> 
    /// to send it to the configured message broker.
    /// </summary>
    /// <param name="executionMode">The execution mode. The default value is <see cref="ExecutionMode.Immediate"/>.</param>
    protected Command(ExecutionMode executionMode = ExecutionMode.Immediate)
        => this.ExecutionMode = executionMode;
}

/// <summary>
/// Represents a command that produces a response.  
/// It can be executed immediately or asynchronously by another service.
/// </summary>
/// <typeparam name="TResponse">The command response type.</typeparam>
public abstract class Command<TResponse> : Contract, ICommand<TResponse>
{
    /// <summary>
    /// Gets the execution mode of the command.  
    /// The default value is <see cref="Abstractions.ExecutionMode.Immediate"/>.
    /// </summary>
    public ExecutionMode ExecutionMode { get; protected set; }

    /// <summary>
    /// Represents the assembly context of the inherited command.
    /// </summary>
    public string ResourceType { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Command{TResponse}"/> class.  
    /// By default, it executes immediately.  
    /// Use <see cref="ExecutionMode.Enqueue"/> to send it to a configured message broker.
    /// </summary>
    /// <param name="executionMode">The execution mode. Default is <see cref="Abstractions.ExecutionMode.Immediate"/>.</param>
    protected Command(ExecutionMode executionMode = ExecutionMode.Immediate)
    {
        this.ExecutionMode = executionMode;
        this.ResourceType = this.GetType().ToString();
    }

    /// <summary>
    /// Configures the command to execute within the current application.
    /// </summary>
    public void PrepareToExecute() => this.ExecutionMode = ExecutionMode.Immediate;
    
    /// <summary>
    /// Configures the command to be enqueued to a message broker and executed by another service.
    /// </summary>
    public void PrepareToSend() => this.ExecutionMode = ExecutionMode.Enqueue;
}