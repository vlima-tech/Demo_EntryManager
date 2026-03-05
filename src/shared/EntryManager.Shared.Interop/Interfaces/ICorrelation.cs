using EntryManager.Shared.Bus.Abstractions;
using Microsoft.Extensions.Primitives;

namespace EntryManager.Shared.Interop;

/// <summary>
/// Represents the correlation context used to propagate identifiers such as
/// TraceId, CorrelationId, and IdempotencyKey across requests, services, and message brokers.
/// </summary>
public interface ICorrelation : IReadOnlyDictionary<string, StringValues>
{
    string GetTraceId();

    string GetTraceParent();
    
    string GetIdempotencyKey();
    
    string GetCorrelationId();

    public string GetMetadata();
    
    /// <summary>
    /// Sets the correlation context based on <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="headers">The <see cref="IDictionary{TKey,TValue}"/> containing HTTP request headers.</param>
    void PropagateContext(IDictionary<string, StringValues> headers);
    
    void PropagateContext(IEnumerable<KeyValuePair<string, StringValues>> headers);
    
    /// <summary>
    /// Sets the correlation context based on the consumed message contract.
    /// </summary>
    /// <param name="message">The <see cref="IDistributedMessage"/> message being processed.</param>
    void PropagateContext(IDistributedMessage message);
}