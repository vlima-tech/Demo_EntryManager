using System.Diagnostics;
using EntryManager.Shared.Bus.Abstractions;
using Microsoft.Extensions.Primitives;

namespace EntryManager.Shared.Interop;

internal class CorrelationContext : Dictionary<string, StringValues>, ICorrelation
{
    public string GetTraceId() => this[KnowHeaders.TraceId].ToString();
    
    public string GetTraceParent() => this[KnowHeaders.TraceParent].ToString();

    public string GetIdempotencyKey() => this[KnowHeaders.IdempotencyKey].ToString(); 

    public string GetCorrelationId() => this[KnowHeaders.CorrelationId].ToString();
    
    public string GetMetadata() => this[KnowHeaders.Metadata].ToString();

    public void PropagateContext(IDistributedMessage message)
        => this.PropagateContext(message.CorrelationKeys);
    
    public void PropagateContext(IDictionary<string, StringValues> headers)
        => this.PropagateContext(headers.AsEnumerable());
    
    public void PropagateContext(IEnumerable<KeyValuePair<string, StringValues>> headers)
    {
        base.Clear();
        
        foreach (var keyValuePair in headers)
            base.Add(keyValuePair.Key, keyValuePair.Value);
        
        this.EnsureEssentialCorrelation();
    }
    
    private void EnsureEssentialCorrelation()
    {
        var correlations = new KeyValuePair<string, Action>[]
        {
            new (KnowHeaders.TraceId, GenerateTraceId),
            new (KnowHeaders.TraceParent, GenerateTraceParent),
            new (KnowHeaders.CorrelationId, GenerateCorrelationId),
            new (KnowHeaders.IdempotencyKey, GenerateIdempotencyKey),
            new (KnowHeaders.Metadata, GenerateMetadata),
        };
        
        foreach (var item in correlations.Where(c => !this.ContainsKey(c.Key)))
            item.Value();

        return;

        void GenerateTraceId()
        {
            var traceId = Activity.Current?.TraceId.ToString();
            
            if (string.IsNullOrWhiteSpace(traceId))
                traceId = Guid.NewGuid().ToString();
            
            this.Add(KnowHeaders.TraceId, traceId);
        }
        
        void GenerateTraceParent()
        {
            var traceParent = Activity.Current?.ParentId;
            
            if (string.IsNullOrWhiteSpace(traceParent))
                traceParent = ActivitySpanId.CreateRandom().ToHexString();
            
            this.Add(KnowHeaders.TraceParent, traceParent);
        }

        void GenerateCorrelationId() => this.Add(KnowHeaders.CorrelationId, Guid.NewGuid().ToString());
        void GenerateIdempotencyKey() => this.Add(KnowHeaders.IdempotencyKey, Guid.NewGuid().ToString());
        void GenerateMetadata() => this.Add(KnowHeaders.Metadata, string.Join(",", string.Empty));
    }
}