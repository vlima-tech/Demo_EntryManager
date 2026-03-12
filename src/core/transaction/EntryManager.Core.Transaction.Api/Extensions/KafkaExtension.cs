using EntryManager.Core.Transaction.Contracts.Events.AccountEvents;
using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;

namespace Microsoft.Extensions.DependencyInjection;

internal static class KafkaExtension
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddKafka(IConfiguration config)
        {
            services.AddKafka(op =>
                {
                    op.ConnectionString = config.GetConnectionString("KafkaConnection")!;
                    op.ProducerConfigSection = config.GetSection("Kafka:Producers");
            
                    op.SetAppNameFromDefaultConvention();
                }).AddProducers(cfg =>
                {
                    cfg.AddProducer<TransactionWasCreatedEvent>();
                    cfg.AddProducer<AccountWasCreatedEvent>();
                })
                .AddConsumers(cfg =>
                {
                    
                }).Build();

            return services;
        }
    }
}