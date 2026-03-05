namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Represents the notification store for the current execution context.  
/// Responsible for storing and retrieving domain notifications, warnings, system errors, and logs.
/// </summary>
public interface INotificationStore
{
    /// <summary>
    /// Retrieves the domain notification messages stored during the current execution process.  
    /// Typically, if notifications exist, the full workflow was not successfully completed.
    /// </summary>
    /// <returns>
    /// A collection of notifications, or an empty collection if none exist.
    /// </returns>
    IEnumerable<Notification> GetNotifications();

    /// <summary>
    /// Retrieves the warning messages stored during the current execution process.  
    /// If no notifications or system errors exist, the workflow completed successfully but may include important warnings.
    /// </summary>
    /// <returns>
    /// A collection of warnings, or an empty collection if none exist.
    /// </returns>
    IEnumerable<Warning> GetWarnings();

    /// <summary>
    /// Retrieves the system error messages stored during the current execution process.  
    /// If system errors exist, the workflow was not successfully completed.  
    /// These messages may contain sensitive information and should not be displayed to users.
    /// </summary>
    /// <returns>
    /// A collection of system errors, or an empty collection if none exist.
    /// </returns>
    IEnumerable<SystemError> GetSystemErrors();

    /// <summary>
    /// Retrieves the log messages stored during the current execution process.
    /// </summary>
    /// <returns>
    /// A collection of log messages, or an empty collection if none exist.
    /// </returns>
    IEnumerable<Log> GetLogs();

    /// <summary>
    /// Retrieves all notifications ordered by time.
    /// </summary>
    /// <param name="includeNotifications">Set to <strong>true</strong> to include domain notifications.</param>
    /// <param name="includeWarnings">Set to <strong>true</strong> to include warnings.</param>
    /// <param name="includeSystemErrors">Set to <strong>true</strong> to include system errors.</param>
    /// <param name="includeLogs">Set to <strong>true</strong> to include log messages.</param>
    /// <returns>
    /// A collection of notifications, or an empty collection if none exist.
    /// </returns>
    IEnumerable<Notification> GetAll(bool includeNotifications = true, bool includeWarnings = true,
        bool includeSystemErrors = true, bool includeLogs = true);

    /// <summary>
    /// Filters notifications based on a predicate and returns them ordered by time.
    /// </summary>
    /// <param name="predicate">A function used to filter each element in the sequence.</param>
    /// <param name="includeNotifications">Set to <strong>true</strong> to include domain notifications.</param>
    /// <param name="includeWarnings">Set to <strong>true</strong> to include warnings.</param>
    /// <param name="includeSystemErrors">Set to <strong>true</strong> to include system errors.</param>
    /// <param name="includeLogs">Set to <strong>true</strong> to include log messages.</param>
    /// <returns>
    /// A collection of filtered notifications, or an empty collection if none match.
    /// </returns>
    IEnumerable<Notification> Find(Func<Notification, bool> predicate, bool includeNotifications = true,
        bool includeWarnings = true, bool includeSystemErrors = true, bool includeLogs = true);

    /// <summary>
    /// Adds a new domain notification to the store.
    /// </summary>
    /// <param name="notification">The notification to add.</param>
    void AddNotification(Notification notification);

    /// <summary>
    /// Adds a new warning to the store.
    /// </summary>
    /// <param name="warning">The warning to add.</param>
    void AddWarning(Warning warning);

    /// <summary>
    /// Adds a new system error to the store.
    /// </summary>
    /// <param name="systemError">The system error to add.</param>
    void AddSystemError(SystemError systemError);

    /// <summary>
    /// Adds a new log message to the store.
    /// </summary>
    /// <param name="log">The log to add.</param>
    void AddLog(Log log);

    /// <summary>
    /// Determines whether the store contains notifications.  
    /// By default, this includes domain notifications and system errors.
    /// </summary>
    /// <param name="includeNotifications">Set to <strong>true</strong> to include <see cref="Notification"/> messages.</param>
    /// <param name="includeWarnings">Set to <strong>true</strong> to include <see cref="Warning"/> messages.</param>
    /// <param name="includeSystemErrors">Set to <strong>true</strong> to include <see cref="SystemError"/> messages.</param>
    /// <returns>
    /// <strong>true</strong> if any notifications exist; otherwise, <strong>false</strong>.
    /// </returns>
    bool HasNotifications(bool includeNotifications = true, bool includeWarnings = false, bool includeSystemErrors = true);

    /// <summary>
    /// Determines whether the store contains log messages.
    /// </summary>
    /// <returns>
    /// <strong>true</strong> if log messages exist; otherwise, <strong>false</strong>.
    /// </returns>
    bool HasLogs();

    /// <summary>
    /// Determines whether the store contains system error messages.
    /// </summary>
    /// <returns>
    /// <strong>true</strong> if system error messages exist; otherwise, <strong>false</strong>.
    /// </returns>
    bool HasSystemErrors();

    /// <summary>
    /// Determines whether the store contains warning messages.
    /// </summary>
    /// <returns>
    /// <strong>true</strong> if warnings exist; otherwise, <strong>false</strong>.
    /// </returns>
    bool HasWarnings();

    /// <summary>
    /// Clears all notification messages from the store.
    /// </summary>
    void Clear();
}