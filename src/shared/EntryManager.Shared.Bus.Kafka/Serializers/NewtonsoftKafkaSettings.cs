using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace EntryManager.Shared.Bus.Kafka.Serializers;

public static class NewtonsoftKafkaSettings
{
    public static JsonSerializerSettings Default => new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        
        TypeNameHandling = TypeNameHandling.Auto,
        SerializationBinder = new DefaultSerializationBinder(), 
        MetadataPropertyHandling = MetadataPropertyHandling.Default,
        
        Converters = new List<JsonConverter> 
        {
            new StringEnumConverter(),
            new VersionConverter(),
            new KeyValuePairConverter(),
            new ContractDataConverter()
        }
    };
}