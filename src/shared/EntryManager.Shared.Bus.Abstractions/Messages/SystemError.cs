using System.Runtime.CompilerServices;
using System.Text;
using EntryManager.Shared.Bus.Abstractions.Validations;
using FluentValidation.Results;

namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Represents a notification event that captures and describes a system error,  
/// including exception details, inner exception, and stack trace information.  
/// </summary>
public class SystemError : Notification
{
    #region Properties

    /// <summary>
    /// Gets the message of the exception that caused the system error.  
    /// </summary>
    public string Exception { get; private set; }

    /// <summary>
    /// Gets the message of the inner exception, if one exists.  
    /// </summary>
    public string InnerException { get; private set; }

    /// <summary>
    /// Gets the stack trace of the exception that caused the system error.  
    /// </summary>
    public string StackTrace { get; private set; }
    
    #endregion

    #region Constructors
    
    /// <summary>
    /// Initializes a new instance of the system error class with the specified error message and exception details.  
    /// </summary>
    /// <param name="errorMessage">The descriptive error message.</param>
    /// <param name="exception">The exception associated with the system error.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    public SystemError(string errorMessage, Exception exception, [CallerMemberName] string sourceMethod = "",
        [CallerLineNumber] int sourceLineNumber = 0, [CallerFilePath] string sourceFileName = "")
        : base(errorMessage, sourceMethod, sourceLineNumber, sourceFileName)
    {
        this.Code = "SYS_ERROR";
        this.Exception = exception?.Message;
        this.StackTrace = exception?.StackTrace;
        this.InnerException = exception?.InnerException?.Message;
    }
    
    /// <summary>
    /// Initializes a new instance of the system error class with the specified error code, message, and exception details.  
    /// </summary>
    /// <param name="errorCode">The code that categorizes the system error.</param>
    /// <param name="errorMessage">The descriptive error message.</param>
    /// <param name="exception">The exception associated with the system error.</param>
    /// <param name="sourceMethod">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceLineNumber">Automatically captured at runtime. Do not set manually.</param>
    /// <param name="sourceFileName">Automatically captured at runtime. Do not set manually.</param>
    public SystemError(string errorCode, string errorMessage, Exception exception,
        [CallerMemberName] string sourceMethod = "", [CallerLineNumber] int sourceLineNumber = 0, 
        [CallerFilePath] string sourceFileName = "") 
        : base(errorCode, errorMessage, sourceMethod, sourceLineNumber, sourceFileName)
    {
        this.Exception = exception?.Message;
        this.StackTrace = exception?.StackTrace;
        this.InnerException = exception?.InnerException?.Message;
    }
    
    #endregion
    
    public override string ToString()
    {
        var fileName = Path.GetFileName(SourceFileName);
        var method = SourceMethod;
        var codePart = $"[{Code}] ";
        var innerPart = $"{Environment.NewLine}Inner Exception: {InnerException}";
        var stackPart = $"{Environment.NewLine}Stack Trace:{Environment.NewLine}{StackTrace}";

        return new StringBuilder()
            .Append($"{codePart}{Message}")
            .AppendLine($"{Environment.NewLine}Exception: {Exception}")
            .AppendLine(innerPart)
            .AppendLine($"{Environment.NewLine}Location: {method} in {fileName}:{SourceLineNumber}")
            .AppendLine(stackPart)
            .ToString();
    }
}