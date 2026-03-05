using System.Runtime.CompilerServices;
using EntryManager.Shared.Bus.Abstractions.Validations;
using FluentValidation.Results;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Represents a notification event that conveys informational, warning, 
/// or domain messages within the application.
/// </summary>
public class Notification : Event
{
    #region Properties

    /// <summary>
    /// Gets the notification code that categorizes the message.
    /// </summary>
    public string? Code { get; protected set; }

    /// <summary>
    /// Gets the notification message content.
    /// </summary>
    public string Message { get; protected set; }

    /// <summary>
    /// Gets the name of the method that instantiated the current notification.  
    /// This value is automatically captured at runtime and should not be set manually.
    /// </summary>
    public string SourceMethod { get; }
    
    /// <summary>
    /// Gets the line number in the source file where the notification was instantiated.  
    /// This value is automatically captured at runtime and should not be set manually.
    /// </summary>
    public int SourceLineNumber { get; }
    
    /// <summary>
    /// Gets the full path of the source file where the notification was instantiated.  
    /// This value is automatically captured at runtime and should not be set manually.
    /// </summary>
    public string SourceFileName { get; }
    
    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the notification class with a specified execution mode.
    /// </summary>
    /// <param name="message">The notification message content.</param>
    /// <param name="executionMode">
    /// The execution mode.  
    /// <see cref="ExecutionMode.Immediate"/> executes immediately.  
    /// <see cref="ExecutionMode.Enqueue"/> enqueues the message if a broker is configured.
    /// </param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    public Notification(string message, ExecutionMode executionMode, [CallerMemberName] string sourceMethod = "",
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "") : base(executionMode)
    {
        this.Message = message;
        this.SourceMethod = sourceMethod;
        this.SourceLineNumber = sourceLineNumber;
        this.SourceFileName = sourceFileName;
    }

    /// <summary>
    /// Initializes a new instance of the notification class with a specific code, message, and execution mode.
    /// </summary>
    /// <param name="code">The notification code.</param>
    /// <param name="message">The notification message content.</param>
    /// <param name="executionMode">
    /// The execution mode.  
    /// <see cref="ExecutionMode.Immediate"/> executes immediately.  
    /// <see cref="ExecutionMode.Enqueue"/> enqueues the message if a broker is configured.
    /// </param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    public Notification(string code, string message, ExecutionMode executionMode, [CallerMemberName] string sourceMethod = "",
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "") 
        : this(message, executionMode, sourceMethod, sourceLineNumber, sourceFileName)
        => this.Code = code;
    
    /// <summary>
    /// Initializes a new instance of the notification class with a specific code, message, and execution mode.
    /// </summary>
    /// <param name="message">The notification message content.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    public Notification(string message, [CallerMemberName] string sourceMethod = "", 
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "") 
        : this(message, ExecutionMode.Immediate, sourceMethod, sourceLineNumber, sourceFileName)
    { }

    /// <summary>
    /// Initializes a new instance of the notification class with a specific code, message, and execution mode.
    /// </summary>
    /// <param name="code">The notification code.</param>
    /// <param name="message">The notification message content.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    public Notification(string code, string message, [CallerMemberName] string sourceMethod = "",
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "") 
        : this(code, message, ExecutionMode.Immediate, sourceMethod, sourceLineNumber, sourceFileName)
    { }
    
    #endregion

    /// <summary>
    /// Determines whether the notification has a code assigned.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if a code is defined; otherwise, <see langword="false"/>.
    /// </returns>
    public bool HasNotificationCode() => !string.IsNullOrEmpty(Code);
    
    /// <summary>
    /// Returns a formatted string containing the notification code (if any) and message.
    /// </summary>
    /// <returns>A string that represents the current notification.</returns>
    public override string ToString()
    {
        var codePart = string.IsNullOrWhiteSpace(Code) ? "" : $"[{Code}] ";
        var fileName = string.IsNullOrWhiteSpace(SourceFileName) ? "unknown file" : Path.GetFileName(SourceFileName);
        var method = string.IsNullOrWhiteSpace(SourceMethod) ? "unknown method" : SourceMethod;

        return $"{codePart}{Message} (Method: {method}, File: {fileName}, Line: {SourceLineNumber})";
    }
}