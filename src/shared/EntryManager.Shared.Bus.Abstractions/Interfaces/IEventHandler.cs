using MediatR;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Defines the abstraction for implementing an event handler that processes a specific event type.
/// </summary>
/// <typeparam name="TEvent">
/// The event type that carries the data related to the notification being handled.
/// </typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>, IDisposable
    where TEvent : IEvent
{
}