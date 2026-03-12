using Microsoft.Extensions.DependencyInjection;

namespace EntryManager.Shared.Data.MongoDB;

public interface IDbContextBuilder
{
    IServiceCollection Seed();
}