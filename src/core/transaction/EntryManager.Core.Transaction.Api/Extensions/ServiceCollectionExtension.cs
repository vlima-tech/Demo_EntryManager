using System.Text.Json.Serialization;

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
            });
            
            services.AddHttpContextAccessor();
            services.AddCorrelation(env.EnvironmentName);
            
            return services;
        }
    }
}