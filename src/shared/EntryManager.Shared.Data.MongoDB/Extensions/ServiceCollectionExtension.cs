using EntryManager.Shared.Data.MongoDB.Builders;
using EntryManager.Shared.Data.MongoDB.Options;
using Microsoft.Extensions.DependencyInjection;

namespace EntryManager.Shared.Data.MongoDB;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IDbContextBuilder AddDbContext<TContext>(Action<DbContextOptions> options, 
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(contextLifetime);
            
            var contextOptions = new DbContextOptions();
            options.Invoke(contextOptions);
            
            switch (optionsLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<DbContextOptions>(contextOptions);
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<DbContextOptions>(provider => contextOptions);
                    break;

                case ServiceLifetime.Scoped:
                default:
                    services.AddScoped(provider => contextOptions);
                    break;
            }
            
            var contextBuilder = new DbContextBuilder<TContext>(services);
            return contextBuilder;
        }

        public void AddDbContext<TContext>(Action<IServiceProvider, DbContextOptions> options,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(contextLifetime);

            switch (optionsLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<DbContextOptions>(provider =>
                    {
                        var op = new DbContextOptions();
                        options.Invoke(provider, op);

                        return op;
                    });
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<DbContextOptions>(provider =>
                    {
                        var op = new DbContextOptions();
                        options.Invoke(provider, op);

                        return op;
                    });
                    break;

                case ServiceLifetime.Scoped:
                default:
                    services.AddScoped<DbContextOptions>(provider =>
                    {
                        var op = new DbContextOptions();
                        options.Invoke(provider, op);

                        return op;
                    });
                    break;
            }
        }

        private void AddDbContext<TContext>(ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            switch (contextLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<TContext>();
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient<TContext>();
                    break;

                case ServiceLifetime.Scoped:
                default:
                    services.AddScoped<TContext>();
                    break;
            }
        }
    }
}