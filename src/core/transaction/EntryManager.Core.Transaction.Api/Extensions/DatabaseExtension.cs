using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Infra.Data.Context;
using EntryManager.Core.Transaction.Api.Infra.Data.Repositories;
using EntryManager.Shared.Data.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Microsoft.Extensions.DependencyInjection;

internal static class DatabaseExtension
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddDatabase(IConfiguration config)
        {
            var connection = config.GetConnectionString("MongoConnection")!;
            
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            
            services.AddDbContext<TransactionContext>(op => op.UseConnectionString(connection))
                .Seed();
            
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountReadRepository, AccountReadRepository>();
            services.AddScoped<IAccountQuery, AccountReadRepository>();
            
            return services;
        }
    }

    extension(IHostApplicationBuilder app)
    {
        internal IHostApplicationBuilder InitializeDatabase()
        {
            return app;
        }
    }
}