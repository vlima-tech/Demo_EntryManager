
using System.Diagnostics;
using System.Reflection;
using EntryManager.Shared.Interop;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCorrelation(string environmentName)
        {
            var app = ObtainsAppInfo(environmentName);
            
            ActivitySource activitySource = new(app.ServiceName, app.Version);

            services.AddSingleton<AppInfo>(app);
            services.AddSingleton(activitySource);
            services.AddScoped<ICorrelation, CorrelationContext>();
            
            return services;
        }

        private static AppInfo ObtainsAppInfo(string environmentName)
        {
            var entryAssembly = Assembly.GetEntryAssembly()!;
            var assemblyFileInfo = new FileInfo(entryAssembly.Location);
            
            var serviceFileName = assemblyFileInfo.Name[assemblyFileInfo.Extension.Length..];
                
            var serviceName = entryAssembly
                .GetCustomAttribute<AssemblyProductAttribute>()
                ?.Product ?? serviceFileName;
            
            var version = entryAssembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>()
                ?.Version ?? "unknown";
            
            var app = new AppInfo
            {
                ServiceName = serviceName,
                Version = version,
                Environment = environmentName
            };
            
            return app;
        }
    }
}