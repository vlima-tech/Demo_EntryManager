using MediatR;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Defines the abstraction for implementing a command handler that executes a command  
/// and returns a boolean value indicating the execution result.
/// </summary>
/// <typeparam name="TCommand">
/// The command type that carries the input data required for execution.
/// </typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, bool>, IDisposable
    where TCommand : ICommand
{
}

/// <summary>
/// Defines the abstraction for implementing a command handler that executes a command  
/// and returns a strongly typed response.
/// </summary>
/// <typeparam name="TCommand">
/// The command type that carries the input data required for execution.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the result returned after command execution.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>, IDisposable
    where TCommand : ICommand<TResponse>
{
}