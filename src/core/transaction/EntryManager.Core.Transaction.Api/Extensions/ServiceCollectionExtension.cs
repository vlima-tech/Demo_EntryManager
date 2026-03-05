using System.Text.Json.Serialization;
using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;
using EntryManager.Shared.Bus.Kafka;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfiguration config, IHostEnvironment env)
        {
            services.AddOpenApi();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            
            services.AddServiceBus(new []
            {
                typeof(Program).Assembly,
                typeof(KafkaDistributedMessageEventHandler).Assembly,
            });

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
            
            services.AddHttpContextAccessor();
            services.AddCorrelation(env.EnvironmentName);
            
            return services;
        }
    }
}