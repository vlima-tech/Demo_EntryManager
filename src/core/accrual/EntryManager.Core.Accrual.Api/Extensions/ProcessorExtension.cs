using EntryManager.Core.Accrual.Api.Core.Processors.TransactionProcessors.RegisterTransactionProcessor;

namespace Microsoft.Extensions.DependencyInjection;

public static class ProcessorExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddProcessors()
        {
            services.AddTransactionProcessors();
            
            return services;
        }

        private void AddTransactionProcessors()
        {
            AddRegisterTransactionProcessor(services);
            
            return;
            
            void AddRegisterTransactionProcessor(IServiceCollection services)
            {
                services.AddScoped<IRegisterTransactionProcessor, UpdateCacheStep>();
                services.Decorate<IRegisterTransactionProcessor, CommitTransactionStep>();
                services.Decorate<IRegisterTransactionProcessor, CheckCategoryStep>();
                services.Decorate<IRegisterTransactionProcessor, CheckGroupStep>();
                services.Decorate<IRegisterTransactionProcessor, CheckAccountStep>();
            }
        }
    }
}