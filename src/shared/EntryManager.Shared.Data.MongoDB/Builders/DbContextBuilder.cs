using Microsoft.Extensions.DependencyInjection;

namespace EntryManager.Shared.Data.MongoDB.Builders;

public class DbContextBuilder<TContext>(IServiceCollection services) : IDbContextBuilder where TContext : DbContext
{
    private readonly IServiceCollection _services = services;

    public IServiceCollection Seed() => this._services;
}