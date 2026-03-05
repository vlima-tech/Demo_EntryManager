using System.Collections.Concurrent;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Bus.Kafka.Helppers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntryManager.Shared.Bus.Kafka
{
    public class KafkaDistributedMessageEventHandler(IServiceProvider provider) : IEventHandler<DistributedMessageEvent>
    {
        private static readonly ConcurrentDictionary<Type, string> _producerNameCache = new();
        private readonly ILogger<KafkaDistributedMessageEventHandler> _logger =  provider.GetRequiredService<ILogger<KafkaDistributedMessageEventHandler>>();
        private readonly IKafkaRegisteredProducerCollection _registeredProducers = provider.GetRequiredService<IKafkaRegisteredProducerCollection>();
        private IKafkaOptionsBuilder _options { get; set; } = provider.GetRequiredService<IKafkaOptionsBuilder>();
        private ITopicProducerProvider _producerProvider { get; set; } = provider.GetRequiredService<ITopicProducerProvider>();
        
        public async Task Handle(DistributedMessageEvent notification, CancellationToken cancellationToken)
        {
            var appName = _options.ApplicationName;
            var dataType = notification.Data.GetType();
            
            var topicName = _producerNameCache.GetOrAdd(dataType, t => ResolveTopicName(appName, dataType));
            var topicAddress = new Uri($"topic:{topicName}");
            
            if(!_registeredProducers.Contains(dataType))
                this._logger.LogWarning("Auto-topic convention applied: {Contract} -> {Topic}", notification.Data.GetType().Name, topicName);
            
            var producer = this._producerProvider.GetProducer<IDistributedMessage>(topicAddress);

            /*
            await producer.Produce(notification, Pipe.Execute<SendContext<DistributedMessageEvent>>(context =>
            {
                context.CorrelationId = Guid.TryParse(notification.CorrelationId, out var guid) ? guid : Guid.Empty;
            }));
            */
            
            await producer.Produce(notification, cancellationToken);
        }

        private static string ResolveTopicName(string appName, Type type)
            => _producerNameCache.GetOrAdd(type, t => TopicNameConventionHelpper.FormatTopicName(appName, type));
        
        public void Dispose()
        {
            this._producerProvider = null;
        }
    }
}