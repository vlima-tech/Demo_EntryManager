using EntryManager.Shared.Bus.Abstractions;
using Microsoft.Extensions.Logging;

namespace EntryManager.Shared.Bus.Core;

/// <summary>
/// Handles the execution of notification events and stores them in the notification store.
/// </summary>
internal class NotificationHandler(ILogger<NotificationHandler> logger, INotificationStore store) 
    : IEventHandler<Notification>, IEventHandler<Warning>, IEventHandler<SystemError>, IEventHandler<Log>
{
    private readonly ILogger<NotificationHandler> _logger = logger;
    private readonly INotificationStore _store = store;

    public Task Handle(Warning warning, CancellationToken cancellationToken)
        => Handle((Notification)warning, cancellationToken);
    
    public Task Handle(SystemError systemError, CancellationToken cancellationToken)
        => Handle((Notification)systemError, cancellationToken);
    
    public Task Handle(Log log, CancellationToken cancellationToken)
        => Handle((Notification)log, cancellationToken);
    
    /// <summary>
    /// Handles the specified notification event by logging it and storing it in the notification store.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        switch (notification)
        {
            case Warning warning:
                _logger?.LogWarning(notification.ToString());
                _store?.AddWarning(warning);
                break;

            case SystemError systemError:
                _logger?.LogError(notification.ToString());
                _store?.AddSystemError(systemError);
                break;

            case Log log:
                _logger?.LogInformation(notification.ToString());
                _store?.AddLog(log);
                break;

            default:
                _logger?.LogInformation(notification.ToString());
                _store?.AddNotification(notification);
                break;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="NotificationHandler"/>  
    /// and clears the associated notification store.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs the actual disposal logic for managed resources.
    /// </summary>
    /// <param name="disposing">A value indicating whether managed resources should be disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        
        _store?.Clear();
    }
}