using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace EntryManager.Shared.Bus.Kafka.Serializers;

public class NewtonsoftKafkaSerializer<T>(JsonSerializerSettings settings) : IAsyncSerializer<T>
{
    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        if (data == null) return Task.FromResult<byte[]>([]);

        var json = JsonConvert.SerializeObject(data, settings);
        
        return Task.FromResult(Encoding.UTF8.GetBytes(json));
    }
}