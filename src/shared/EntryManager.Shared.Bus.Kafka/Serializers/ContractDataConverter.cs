using EntryManager.Shared.Bus.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EntryManager.Shared.Bus.Kafka.Serializers;

public class ContractDataConverter : JsonConverter<IContract>
{
    private static readonly Dictionary<string, Type> _knownTypes;

    static ContractDataConverter()
    {
        // Mapeia todos os tipos concretos que implementam IContract
        _knownTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return []; } // ignora assemblies dinâmicos inválidos
            })
            .Where(t => typeof(IContract).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToDictionary(t => t.FullName!, StringComparer.OrdinalIgnoreCase);
    }

    public override IContract ReadJson(JsonReader reader, Type objectType, IContract existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var targetType = ObtainsResourceType(jsonObject);

        // Criar serializer sem este converter para evitar loop
        var innerSerializer = CreateInnerSerializer(serializer);
        
        using var subReader = jsonObject.CreateReader();
        return (IContract)innerSerializer.Deserialize(subReader, targetType);
    }

    public override void WriteJson(JsonWriter writer, IContract? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        CreateInnerSerializer(serializer)
            .Serialize(writer, value);
    }
    
    private static JsonSerializer CreateInnerSerializer(JsonSerializer serializer)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = serializer.ContractResolver,
            NullValueHandling = serializer.NullValueHandling,
            DateFormatHandling = serializer.DateFormatHandling,
            // Mantemos o Auto para o Producer gravar o $type, 
            // mas o ReadJson usa o targetType explicitamente
            TypeNameHandling = serializer.TypeNameHandling, 
            Converters = serializer.Converters.Where(c => c.GetType() != typeof(ContractDataConverter)).ToList()
        };
        return JsonSerializer.Create(settings);
    }
    
    private static Type ObtainsResourceType(JObject jObject)
    {
        var typeName = jObject["$type"]?.ToString() ?? jObject["resourceType"]?.ToString();
        
        var targetTypeName = typeName?.Split(',').FirstOrDefault();
        
        if(string.IsNullOrEmpty(targetTypeName))
            throw new JsonSerializationException("Cannot determine concrete contract type — missing '$type' or 'resourceType' property.");

        return _knownTypes.TryGetValue(targetTypeName, out var targetType) 
            ? targetType 
            : throw new JsonSerializationException($"Unknown contract type '{typeName}' — cannot resolve a concrete implementation for IContract.");
    }
}