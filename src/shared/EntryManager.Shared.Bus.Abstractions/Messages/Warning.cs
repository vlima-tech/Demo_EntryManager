using System.Runtime.CompilerServices;

namespace EntryManager.Shared.Bus.Abstractions;

public class Warning : Notification
{
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
    public Warning(string message, ExecutionMode executionMode, [CallerMemberName] string sourceMethod = "",
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "") 
        : base(message, executionMode, sourceMethod, sourceLineNumber, sourceFileName)
    { }

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
    public Warning(string code, string message, ExecutionMode executionMode, [CallerMemberName] string sourceMethod = "",
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "") 
        : base(code, message, executionMode, sourceMethod, sourceLineNumber, sourceFileName)
    { }
    
    /// <summary>
    /// Initializes a new instance of the notification class with a specific code, message, and execution mode.
    /// </summary>
    /// <param name="message">The notification message content.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    public Warning(string message, [CallerMemberName] string sourceMethod = "", 
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
    public Warning(string code, string message, [CallerMemberName] string sourceMethod = "",
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "") 
        : this(code, message, ExecutionMode.Immediate, sourceMethod, sourceLineNumber, sourceFileName)
    { }
    
    #endregion
}