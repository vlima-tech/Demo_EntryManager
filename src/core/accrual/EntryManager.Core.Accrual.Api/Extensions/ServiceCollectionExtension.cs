using EntryManager.Shared.Bus.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfiguration config, IHostEnvironment env)
        {
            services.AddMvcEssential();
            
            services.AddServiceBus(new []
            {
                typeof(Program).Assembly,
                typeof(KafkaDistributedMessageEventHandler).Assembly,
            });

            services.AddKafka(config);
            services.AddDatabase(config);
            services.AddCorrelation(env.EnvironmentName);
            services.AddProcessors();
            services.AddCache(config);
            
            return services;
        }

        private IServiceCollection AddMvcEssential()
        {
            services.AddOpenApi();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
            
            // desabilita validação automática asp.net core de NRT's (Nullable Reference Types)
            services.Configure<ApiBehaviorOptions>(op =>
            {
                op.SuppressModelStateInvalidFilter = true;
            });
            
            services.AddHttpContextAccessor();

            return services;
        }
    }
}