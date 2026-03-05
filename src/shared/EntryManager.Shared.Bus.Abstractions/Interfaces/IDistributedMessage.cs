using Microsoft.Extensions.Primitives;

namespace EntryManager.Shared.Bus.Abstractions;

public interface IDistributedMessage : IEvent
{
    IDictionary<string, StringValues> CorrelationKeys { get; }
    
    IDictionary<string, string> Metadata { get; }
    
    IContract Data { get; }
}