using System.Diagnostics;
using EntryManager.Shared.Bus.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace EntryManager.Shared.Bus.Core;

internal static class ValidationHelpper
{
    private static IEnumerable<IValidator> ScanValidators(IServiceProvider provider, IContract contract)
    {
        var validatorType = typeof(IEnumerable<>)
            .MakeGenericType(typeof(IValidator<>)
            .MakeGenericType(contract.GetType()));

        return provider.GetService(validatorType) is not IEnumerable<IValidator> validators
            ? []
            : validators;
    }

    internal static async Task<ICollection<ValidationFailure>> ValidateAsync(IServiceProvider provider, IContract contract)
    {
        var logger = provider.GetService<ILogger<ServiceBus>>()!;
        var activitySource = provider.GetService<ActivitySource>()!;
        List<ValidationFailure> errors = [];
        
        var activityName = $"{contract.ContractName} Validation";
        using var activity = activitySource.CreateActivity(activityName, ActivityKind.Internal) ?? new Activity(activityName);
        activity.Start();
        
        var validators = ScanValidators(provider, contract).ToList();

        logger.LogInformation("validators: {ValidatorsCount} - {Join}", 
            validators.Count, string.Join(',', validators.Select(v => v.GetType().Name)));
        
        var validationContext = Activator.CreateInstance(typeof(ValidationContext<>)
            .MakeGenericType(contract.GetType()), contract) as IValidationContext;

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(validationContext);
            
            if (result.IsValid) continue;
            
            errors.AddRange(result.Errors);
            logger.LogError("{Validator} failed: {Join}", validator.GetType().Name, string.Join(",", result.Errors));
        }

        var status = errors.Count > 0 ? ActivityStatusCode.Error : ActivityStatusCode.Ok;

        activity.SetStatus(status);
        activity.Stop();
        
        return errors;
    }
}