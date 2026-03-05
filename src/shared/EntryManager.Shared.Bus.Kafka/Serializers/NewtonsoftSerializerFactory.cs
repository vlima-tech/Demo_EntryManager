using System.Net.Mime;
using Confluent.Kafka;
using MassTransit.KafkaIntegration.Serializers;

namespace EntryManager.Shared.Bus.Kafka.Serializers;

public class NewtonsoftSerializerFactory : IKafkaSerializerFactory
{
    // O Content-Type que o MassTransit espera para JSON
    public ContentType ContentType => new("application/vnd.masstransit+json");

    public IDeserializer<T> GetDeserializer<T>() => new NewtonsoftKafkaDeserializer<T>(NewtonsoftKafkaSettings.Default);
    
    public IAsyncSerializer<T> GetSerializer<T>() => new NewtonsoftKafkaSerializer<T>(NewtonsoftKafkaSettings.Default);
}