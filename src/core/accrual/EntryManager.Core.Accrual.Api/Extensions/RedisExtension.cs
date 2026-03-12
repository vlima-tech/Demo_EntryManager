using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Core.Accrual.Api.Infra.Data.Repositories;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCache(IConfiguration config)
        {
            var redisConnection = config.GetConnectionString("RedisConnection");
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "entrymanager-core-accrual";
            });
            
            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(redisConnection));
            
            return services;
        }
    }
}