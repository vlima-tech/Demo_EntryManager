using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace EntryManager.Shared.Bus.Kafka.Serializers;

public class NewtonsoftKafkaDeserializer<T>(JsonSerializerSettings settings) : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty) return default;

        var json = Encoding.UTF8.GetString(data);

        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}