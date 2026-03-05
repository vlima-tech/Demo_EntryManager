using System.Reflection;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Bus.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddServiceBus(params Assembly[] assemblies)
        {
            // Add Mediator Service
            services.AddMediatR(op =>
            {
                op.RegisterServicesFromAssembly(typeof(NotificationStore).Assembly);
                op.RegisterServicesFromAssemblies(assemblies);
            });
            
            // Service Bus Core
            services.AddScoped<IServiceBus, ServiceBus>();

            // Notification Store
            services.AddScoped<List<Log>>();
            services.AddScoped<List<Notification>>();
            services.AddScoped<List<Warning>>();
            services.AddScoped<List<SystemError>>();
            services.AddScoped<INotificationStore, NotificationStore>();
        }
    }
}