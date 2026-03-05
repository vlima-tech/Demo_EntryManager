namespace EntryManager.Shared.Bus.Abstractions;

/// <summary>
/// Represents the base abstraction for contracts or messages.
/// Use <see cref="ICommand"/> or <see cref="IEvent"/> interfaces, or 
/// <see cref="Command"/> and <see cref="Event"/> base classes, when implementing 
/// commands or events within the application.
/// </summary>
public interface IContract
{
    /// <summary>
    /// Gets the unique identifier of the contract.
    /// </summary>
    Guid ContractId { get; }
    
    /// <summary>
    /// Gets the idempotency key used to ensure the operation is not processed more than once.
    /// </summary>
    string IdempotencyKey { get; }
    
    /// <summary>
    /// Gets the correlation identifier used by the caller to link this contract 
    /// with other processes in a workflow.
    /// Use it as the <c>x-correlation-id</c> header name in HTTP calls, or within 
    /// observability tools such as 
    /// <a href="https://opentelemetry.io/docs/instrumentation/net/getting-started">OpenTelemetry</a> 
    /// for troubleshooting and tracing.  
    /// See more 
    /// <a href="https://microsoft.github.io/code-with-engineering-playbook/observability/correlation-id">here</a>.
    /// </summary>
    string CorrelationId { get; }
    
    /// <summary>
    /// Gets the trace identifier used in observability tools such as 
    /// <a href="https://opentelemetry.io/docs/instrumentation/net/getting-started">OpenTelemetry</a> 
    /// for distributed tracing and troubleshooting.
    /// </summary>
    string TraceId { get; }
    
    /// <summary>
    /// Gets the contract name, which is automatically generated based on the 
    /// implementing <see cref="Command"/> or <see cref="Event"/> class name.  
    /// For example, <c>CreateCustomerCommand</c> results in <c>CREATE_CUSTOMER</c>, 
    /// and <c>CustomerWasCreatedEvent</c> results in <c>CUSTOMER_WAS_CREATED</c>.
    /// </summary>
    string ContractName { get; }
    
    /// <summary>
    /// Gets the date and time when the contract instance was created.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the contract type, automatically determined from the implementation.  
    /// Possible values are <see cref="ContractType.Command"/> and <see cref="ContractType.Event"/>.
    /// </summary>
    ContractType ContractType { get; }

    bool IsCorrelated();
    
    bool IsNotCorrelated();
    
    void CorrelateTo(IContract contract);
    
    void CorrelateTo(string traceId);
    
    void CorrelateTo(string traceId, string correlationId);
    
    void CorrelateTo(string traceId, string correlationId, string idempotencyKey);
}