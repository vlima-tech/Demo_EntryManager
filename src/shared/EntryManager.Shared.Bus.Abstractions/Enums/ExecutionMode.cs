
namespace EntryManager.Shared.Bus.Abstractions;

public enum ExecutionMode
{
    /// <summary>
    /// Executes locally when sent through the service bus.
    /// </summary>
    Immediate = 0,

    /// <summary>
    /// Enqueues the message to the configured message broker for asynchronous execution.
    /// </summary>
    Enqueue = 1,
}