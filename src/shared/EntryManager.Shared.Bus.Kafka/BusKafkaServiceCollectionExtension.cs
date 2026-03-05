using System.Net.Mime;
using Confluent.Kafka;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Bus.Kafka;
using EntryManager.Shared.Bus.Kafka.Helppers;
using EntryManager.Shared.Bus.Kafka.Serializers;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class BusKafkaServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IKafkaBuilder AddKafka(Action<IKafkaOptionsBuilder> options)
        {
            IKafkaOptionsBuilder optionsBuilder = new KafkaOptionsBuilder();
            
            services.AddSingleton(optionsBuilder);
            
            options(optionsBuilder);
            
            return new KafkaBuilder(services, optionsBuilder);
        }
    }
}

public interface IKafkaOptionsBuilder
{
    string ConnectionString { get; set; }
    
    string ApplicationName { get; }
    
    void SetAppName(string appName);
    
    void SetAppNameFromEnvironment(string varName = "APP_NAME");

    void SetAppNameFromDefaultConvention();

    IConfigurationSection ProducerConfigSection { get; set; }
    
    IConfigurationSection ConsumerConfigSection { get; set; }
}

internal class KafkaOptionsBuilder : IKafkaOptionsBuilder
{
    public string ConnectionString { get; set; }
    
    public string ApplicationName { get; private set; }
    
    public void SetAppName(string appName) => this.ApplicationName = appName;

    public void SetAppNameFromEnvironment(string variableName = "APP_NAME")
        => this.ApplicationName = Environment.GetEnvironmentVariable(variableName) ?? string.Empty;

    public void SetAppNameFromDefaultConvention()
        => this.ApplicationName = Normalize(GetAssemblyFallback());
    
    public IConfigurationSection ProducerConfigSection { get; set; }
    
    public IConfigurationSection ConsumerConfigSection { get; set; }
    
    private static string GetAssemblyFallback() =>
        System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown-app";

    private static string Normalize(string value) => value.ToLower().Replace(" ", "-").Replace(".", "-");
}

/// <summary>
/// Builder interno para configuração fluente de producers Kafka.
/// </summary>
public interface IKafkaProducerBuilder
{
    IKafkaProducerBuilder AddProducer<T>() where T : class, IContract;
}

internal class KafkaProducerBuilder : IKafkaProducerBuilder
{
    private readonly RegisteredProducerCollection _registeredProducers = [];
    
    public IKafkaProducerBuilder AddProducer<T>() where T : class, IContract
    {
        this._registeredProducers.Add(typeof(T));
        
        return this;
    }

    public IKafkaRegisteredProducerCollection Build() => this._registeredProducers;
}

public interface IKafkaConsumerBuilder
{
    IKafkaConsumerBuilder AddCommandConsumer<TCommand>()
        where TCommand : class, ICommand;
    
    IKafkaConsumerBuilder AddCommandConsumer<TCommand, TResponse, THandler>()
        where TCommand : class, ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>;
    
    IKafkaConsumerBuilder AddEventConsumer<TEvent>()
        where TEvent : class, IEvent;
}

internal class KafkaConsumerBuilder : IKafkaConsumerBuilder
{
    private readonly RegisteredConsumerCollection _registeredConsumers = [];
    
    public IKafkaConsumerBuilder AddCommandConsumer<TCommand>() 
        where TCommand : class, ICommand
    {
        _registeredConsumers.Add(typeof(TCommand));
        return this;
    }

    public IKafkaConsumerBuilder AddCommandConsumer<TCommand, TResponse, THandler>() 
        where TCommand : class, ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
    {
        throw new NotImplementedException();
    }

    public IKafkaConsumerBuilder AddEventConsumer<TEvent>() 
        where TEvent : class, IEvent
    {
        _registeredConsumers.Add(typeof(TEvent));
        return this;
    }
    
    public IKafkaRegisteredConsumerCollection Build() => this._registeredConsumers;
}

public interface IKafkaBuilder
{
    IKafkaBuilder AddProducers(Action<IKafkaProducerBuilder> configure);
    IKafkaBuilder AddConsumers(Action<IKafkaConsumerBuilder> configure);
    IServiceCollection Build();
}

internal class KafkaBuilder(IServiceCollection services, IKafkaOptionsBuilder options) : IKafkaBuilder
{
    private readonly IKafkaOptionsBuilder _options = options;
    private IKafkaRegisteredProducerCollection _registeredProducers { get; set; } = new RegisteredProducerCollection();
    private IKafkaRegisteredConsumerCollection _registeredConsumers { get; set; } = new RegisteredConsumerCollection();
    
    public IKafkaBuilder AddProducers(Action<IKafkaProducerBuilder> configure)
    {
        var producerBuilder = new KafkaProducerBuilder();
        
        configure(producerBuilder);
        
        this._registeredProducers = producerBuilder.Build();
        services.AddSingleton(this._registeredProducers);

        return this;
    }

    public IKafkaBuilder AddConsumers(Action<IKafkaConsumerBuilder> configure)
    {
        var consumerBuilder = new KafkaConsumerBuilder();
        
        configure(consumerBuilder);
        
        this._registeredConsumers = consumerBuilder.Build();
        services.AddSingleton(this._registeredConsumers);
        
        return this;
    }

    public IServiceCollection Build()
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
            
            busConfig.AddRider(rider =>
            {
                rider.AddConsumer<DistributedMessageEventConsumer>();
                
                rider.UsingKafka((context, cfg) =>
                {
                    cfg.Host(options.ConnectionString);
                    cfg.SetSerializationFactory(new NewtonsoftSerializerFactory());
                    
                    foreach (var consumerType in this._registeredConsumers)
                    {
                        var appName = _options.ApplicationName;

                        var topicName = TopicNameConventionHelpper.FormatTopicName(appName, consumerType);
                        
                        var consumerConfig = new ConsumerConfig()
                        {
                            GroupId = $"{topicName}-group",
                            EnableAutoCommit = true,
                            AutoCommitIntervalMs = 2500,
                            EnableAutoOffsetStore = true
                        };
                        
                        cfg.TopicEndpoint<DistributedMessageEvent>(topicName: topicName, consumerConfig, configure: e =>
                        {
                            e.SerializerContentType = new ContentType("application/vnd.masstransit+json");
                            e.DefaultContentType = new ContentType("application/vnd.masstransit+json");
                            
                            e.AutoStart = true;
                            e.CreateIfMissing();
                            
                            e.CheckpointInterval = TimeSpan.FromSeconds(1);
                            e.CheckpointMessageCount = 10;
                            
                            e.ConfigureConsumer<DistributedMessageEventConsumer>(context);
                        });
                    }
                    
                });
            });
        });
        
        return services;
    }
}