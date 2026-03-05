using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Shared.Bus.Core;

/// <summary>
/// Represents the notification store for the current execution context.  
/// Responsible for storing domain notifications, warnings, system errors, and logs generated during execution.
/// </summary>
public class NotificationStore : INotificationStore
{
    private readonly List<Notification> _notifications;
    private readonly List<Warning> _warnings;
    private readonly List<SystemError> _systemErrors;
    private readonly List<Log> _logs;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationStore"/> class.
    /// </summary>
    public NotificationStore()
    {
        _notifications = new List<Notification>();
        _warnings = new List<Warning>();
        _systemErrors = new List<SystemError>();
        _logs = new List<Log>();
    }

    /// <summary>
    /// Retrieves all domain notification messages from the current execution context.
    /// </summary>
    /// <returns>
    /// A collection of domain notifications.
    /// </returns>
    public IEnumerable<Notification> GetNotifications() => _notifications;

    /// <summary>
    /// Retrieves all system error messages from the current execution context.
    /// </summary>
    /// <returns>
    /// A collection of system error messages.
    /// </returns>
    public IEnumerable<SystemError> GetSystemErrors() => _systemErrors;

    /// <summary>
    /// Retrieves all warning messages from the current execution context.
    /// </summary>
    /// <returns>
    /// A collection of warnings.
    /// </returns>
    public IEnumerable<Warning> GetWarnings() => _warnings;

    /// <summary>
    /// Retrieves all log messages from the current execution context.
    /// </summary>
    /// <returns>
    /// A collection of log messages.
    /// </returns>
    public IEnumerable<Log> GetLogs() => _logs;

    /// <summary>
    /// Retrieves all messages in the store based on the specified filters.  
    /// Results are ordered by message time.
    /// </summary>
    /// <param name="includeNotifications">Set to <strong>true</strong> to include domain notifications.</param>
    /// <param name="includeWarnings">Set to <strong>true</strong> to include warnings.</param>
    /// <param name="includeSystemErrors">Set to <strong>true</strong> to include system errors.</param>
    /// <param name="includeLogs">Set to <strong>true</strong> to include log messages.</param>
    /// <returns>
    /// A collection of messages matching the selected filters.
    /// </returns>
    public IEnumerable<Notification> GetAll(bool includeNotifications = true, bool includeWarnings = true,
        bool includeSystemErrors = true, bool includeLogs = true)
    {
        var collection = new List<Notification>();

        if (includeNotifications)
            collection.AddRange(_notifications);

        if (includeWarnings)
            collection.AddRange(_warnings);

        if (includeSystemErrors)
            collection.AddRange(_systemErrors);

        if (includeLogs)
            collection.AddRange(_logs);

        return collection.OrderBy(n => n.CreatedAt).ToList();
    }

    /// <summary>
    /// Filters messages in the notification store based on a predicate.  
    /// Results are ordered by message time.
    /// </summary>
    /// <param name="predicate">A function to filter messages by condition.</param>
    /// <param name="includeNotifications">Include domain notifications in the query.</param>
    /// <param name="includeWarnings">Include warnings in the query.</param>
    /// <param name="includeSystemErrors">Include system errors in the query.</param>
    /// <param name="includeLogs">Include logs in the query.</param>
    /// <returns>
    /// A filtered collection of messages matching the predicate.
    /// </returns>
    public IEnumerable<Notification> Find(Func<Notification, bool> predicate, bool includeNotifications = true,
        bool includeWarnings = true, bool includeSystemErrors = true, bool includeLogs = true)
    {
        var collection = new List<Notification>();

        if (includeNotifications)
            collection.AddRange(_notifications.Where(predicate));

        if (includeWarnings)
            collection.AddRange(_warnings.Where(predicate));

        if (includeSystemErrors)
            collection.AddRange(_systemErrors.Where(predicate));

        if (includeLogs)
            collection.AddRange(_logs.Where(predicate));

        return collection.OrderBy(n => n.CreatedAt).ToList();
    }

    /// <summary>
    /// Adds a new domain notification to the store.
    /// </summary>
    /// <param name="notification">The notification to add.</param>
    public void AddNotification(Notification notification) => _notifications.Add(notification);

    /// <summary>
    /// Adds a new warning to the store.
    /// </summary>
    /// <param name="warning">The warning to add.</param>
    public void AddWarning(Warning warning) => _warnings.Add(warning);

    /// <summary>
    /// Adds a new system error to the store.
    /// </summary>
    /// <param name="systemError">The system error to add.</param>
    public void AddSystemError(SystemError systemError) => _systemErrors.Add(systemError);

    /// <summary>
    /// Adds a new log message to the store.
    /// </summary>
    /// <param name="log">The log to add.</param>
    public void AddLog(Log log) => _logs.Add(log);

    /// <summary>
    /// Determines whether the store contains any messages.  
    /// By default, checks for notifications and system errors.
    /// </summary>
    /// <param name="includeNotifications">Set to <strong>true</strong> to include notifications.</param>
    /// <param name="includeWarnings">Set to <strong>true</strong> to include warnings.</param>
    /// <param name="includeSystemErrors">Set to <strong>true</strong> to include system errors.</param>
    /// <returns>
    /// <strong>true</strong> if any messages exist; otherwise, <strong>false</strong>.
    /// </returns>
    public bool HasNotifications(
        bool includeNotifications = true,
        bool includeWarnings = false,
        bool includeSystemErrors = true)
    {
        if (includeNotifications && _notifications.Count > 0) return true;
        if (includeWarnings && _warnings.Count > 0) return true;
        if (includeSystemErrors && _systemErrors.Count > 0) return true;
        return false;
    }

    /// <summary>
    /// Determines whether the store contains log messages.
    /// </summary>
    /// <returns>
    /// <strong>true</strong> if log messages exist; otherwise, <strong>false</strong>.
    /// </returns>
    public bool HasLogs() => _logs.Count > 0;

    /// <summary>
    /// Determines whether the store contains system error messages.
    /// </summary>
    /// <returns>
    /// <strong>true</strong> if system errors exist; otherwise, <strong>false</strong>.
    /// </returns>
    public bool HasSystemErrors() => _systemErrors.Count > 0;

    /// <summary>
    /// Determines whether the store contains warning messages.
    /// </summary>
    /// <returns>
    /// <strong>true</strong> if warnings exist; otherwise, <strong>false</strong>.
    /// </returns>
    public bool HasWarnings() => _warnings.Count > 0;

    /// <summary>
    /// Clears all messages from the notification store.
    /// </summary>
    public void Clear()
    {
        _notifications.Clear();
        _warnings.Clear();
        _systemErrors.Clear();
        _logs.Clear();
    }
}