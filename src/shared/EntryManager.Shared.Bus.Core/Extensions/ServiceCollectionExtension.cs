using System.Reflection;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Bus.Core;
using FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServiceBus(params Assembly[] assemblies)
        {
            services.AddServiceBusTunnel(assemblies);
            services.AddValidators(assemblies);
            services.AddNotificationStore();
            
            return services;
        }

        private void AddNotificationStore()
        {
            services.AddScoped<List<Log>>();
            services.AddScoped<List<Notification>>();
            services.AddScoped<List<Warning>>();
            services.AddScoped<List<SystemError>>();
            services.AddScoped<INotificationStore, NotificationStore>();
        }

        private void AddServiceBusTunnel(IEnumerable<Assembly> assemblies)
        {
            services.AddMediatR(op =>
            {
                op.RegisterServicesFromAssembly(typeof(NotificationStore).Assembly);
                op.RegisterServicesFromAssemblies(assemblies.ToArray());
            });
            
            services.AddScoped<IServiceBus, ServiceBus>();
        }
        
        private void AddValidators(IEnumerable<Assembly> assemblies)
            => services.AddValidatorsFromAssemblies(assemblies.ToArray());
    }
}