using Microsoft.Extensions.Primitives;

namespace EntryManager.Shared.Bus.Abstractions;

public class DistributedMessageEvent : Event, IDistributedMessage
{
    public IDictionary<string, StringValues> CorrelationKeys { get; private set; }
    public IDictionary<string, string> Metadata { get; private set; }
    public IContract Data { get; set; }
    
    protected DistributedMessageEvent()
    {
        this.CorrelationKeys = new Dictionary<string, StringValues>();
        this.Metadata = new Dictionary<string, string>();
    }
    
    public DistributedMessageEvent(IContract data, IDictionary<string, StringValues> correlationKeys) : this()
    {
        this.Data = data;
        this.CorrelationKeys = correlationKeys;
    }
    
    public DistributedMessageEvent(IContract data, IDictionary<string, StringValues> correlationKeys, 
        IDictionary<string, string> metadata) : this()
    {
        this.Data = data;
        this.CorrelationKeys = correlationKeys;
        this.Metadata = metadata;
    }
}