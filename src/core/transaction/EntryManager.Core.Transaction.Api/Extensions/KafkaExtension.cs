using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
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
                    cfg.AddProducer<CreateTransactionCommand>();
                    cfg.AddProducer<TransactionWasCreatedEvent>();
                })
                .AddConsumers(cfg =>
                {
                    cfg.AddCommandConsumer<CreateTransactionCommand>();
                    cfg.AddEventConsumer<TransactionWasCreatedEvent>();
                }).Build();

            return services;
        }
    }
}