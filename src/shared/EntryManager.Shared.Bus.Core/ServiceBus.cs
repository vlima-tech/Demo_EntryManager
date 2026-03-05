using System.Diagnostics;
using System.Runtime.CompilerServices;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Interop;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntryManager.Shared.Bus.Core;

/// <summary>
/// Provides a wrapper implementation around <see href="https://github.com/LuckyPennySoftware/MediatR">MediatR</see>
/// that offers an abstraction layer for event-driven programming.  
/// It simplifies the use of message brokers, parallel work processing, and distributed communication patterns.
/// </summary>
public sealed class ServiceBus(IServiceProvider provider, IMediator mediator, INotificationStore store) : IServiceBus
{
    private readonly IServiceProvider _provider = provider;
    private readonly IMediator _mediator = mediator;
    private readonly INotificationStore _store = store;
    private readonly ICorrelation _correlation = provider.GetService<ICorrelation>()!;
    private readonly ILogger _logger = provider.GetService<ILogger<ServiceBus>>()!;
    private readonly ActivitySource _otel = provider.GetService<ActivitySource>()!;
    
    public async Task<bool> PublishAsync(IEvent @event, CancellationToken cancellationToken, 
        [CallerMemberName] string sourceMethod = "", [CallerLineNumber] int sourceLineNumber = 0, 
        [CallerFilePath] string sourceFileName = "")
    {
        var traceId = this._correlation.GetTraceId();
        var correlationId = this._correlation.GetCorrelationId();
        var idempotencyKey = this._correlation.GetIdempotencyKey();
        
        @event.CorrelateTo(traceId, correlationId, idempotencyKey);
        
        this._logger.LogInformation("{EventName} execution started.", @event.ContractName);
        using var activity = _otel.StartActivity(@event.ContractName) ?? new Activity(@event.ContractName);
        
        activity.Start();
        
        var validationResult = await ValidationHelpper.ValidateAsync(this._provider, @event);
        
        if (validationResult.Count > 0)
        {
            await this.NotifyValidationFailuresAsync(validationResult, cancellationToken, sourceMethod, 
                sourceLineNumber, sourceFileName);
            
            this._logger.LogInformation("{EventName} execution completed with errors.", @event.ContractName);
            activity.SetStatus(ActivityStatusCode.Error, $"{@event.ContractName} validation failed.");
            activity.Stop();
            
            return false;
        }

        if (@event.ExecutionMode == ExecutionMode.Enqueue)
            await this.EnqueueDistributedMessage(@event, cancellationToken,  sourceMethod, sourceLineNumber, sourceFileName);
        else
        {
            await this.ExecuteOnSandboxAsync(
                @event, 
                async () => await this._mediator.Publish(@event, cancellationToken),
                sourceMethod, sourceLineNumber, sourceFileName
            );
        }
        
        var activityStatus = ActivityStatusCode.Ok;
        
        if (this._store.HasNotifications())
            activityStatus = ActivityStatusCode.Error;
        
        this._logger.LogInformation("{EventName} execution completed.", @event.ContractName);
        activity.SetStatus(activityStatus);
        activity.Stop();
        
        return !this._store.HasNotifications();
    }

    public async Task<bool> SendAsync(ICommand command, CancellationToken cancellationToken, 
        [CallerMemberName] string sourceMethod = "", [CallerLineNumber] int sourceLineNumber = 0, 
        [CallerFilePath] string sourceFileName = "")
    {
        var traceId = this._correlation.GetTraceId();
        var correlationId = this._correlation.GetCorrelationId();
        var idempotencyKey = this._correlation.GetIdempotencyKey();
        
        command.CorrelateTo(traceId, correlationId, idempotencyKey);
        
        this._logger.LogInformation("{CommandName} execution started.", command.ContractName);
        using var activity = _otel.StartActivity(command.ContractName) ?? new Activity(command.ContractName);
        activity.Start();
        
        var validationResult = await ValidationHelpper.ValidateAsync(this._provider, command);
        
        if (validationResult.Count > 0)
        {
            await this.NotifyValidationFailuresAsync(validationResult, cancellationToken, sourceMethod,
                sourceLineNumber, sourceFileName);
            
            this._logger.LogInformation("{CommandName} execution completed with errors.", command.ContractName);
            activity.SetStatus(ActivityStatusCode.Error, $"{command.ContractName} validation failed.");
            activity.Stop();
            
            return false;
        }

        this._logger.LogInformation("{CommandName} validated, preparing to execute.", command.ContractName);
        
        if (command.ExecutionMode == ExecutionMode.Enqueue)
            return await this.EnqueueDistributedMessage(command, cancellationToken,  sourceMethod, sourceLineNumber, sourceFileName);
        
        var result = await this.ExecuteOnSandboxAsync(
            command, 
            async () => await this._mediator.Send(command, cancellationToken),
            sourceMethod, sourceLineNumber, sourceFileName
        );

        var activityStatus = ActivityStatusCode.Ok;
        
        if (this._store.HasNotifications())
            activityStatus = ActivityStatusCode.Error;
        
        this._logger.LogInformation("{CommandName} execution completed.", command.ContractName);
        activity.SetStatus(activityStatus);
        activity.Stop();
        
        return result;
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken, 
        [CallerMemberName] string sourceMethod = "", [CallerLineNumber] int sourceLineNumber = 0, 
        [CallerFilePath] string sourceFileName = "")
    {
        var traceId = this._correlation.GetTraceId();
        var correlationId = this._correlation.GetCorrelationId();
        var idempotencyKey = this._correlation.GetIdempotencyKey();
        
        command.CorrelateTo(traceId, correlationId, idempotencyKey);
        
        this._logger.LogInformation("{CommandName} execution started.", command.ContractName);
        using var activity = _otel.StartActivity(command.ContractName) ?? new Activity(command.ContractName);
        activity.Start();
        
        var validationResult = await ValidationHelpper.ValidateAsync(this._provider, command);
        
        if (validationResult.Count > 0)
        {
            await this.NotifyValidationFailuresAsync(validationResult, cancellationToken, sourceMethod,
                sourceLineNumber, sourceFileName);
            
            this._logger.LogInformation("{CommandName} execution completed with errors.", command.ContractName);
            activity.SetStatus(ActivityStatusCode.Error, $"{command.ContractName} validation failed.");
            activity.Stop();
            
            return default(TResponse);
        }

        if (command.ExecutionMode == ExecutionMode.Enqueue)
        {
            await this.EnqueueDistributedMessage(command, cancellationToken, sourceMethod, sourceLineNumber, sourceFileName);
            
            return default(TResponse);
        }
        
        var result = await this.ExecuteOnSandboxAsync(
            command, 
            async () => await this._mediator.Send(command, cancellationToken),
            sourceMethod, sourceLineNumber, sourceFileName
        );

        var activityStatus = ActivityStatusCode.Ok;
        
        if (this._store.HasNotifications())
            activityStatus = ActivityStatusCode.Error;
        
        this._logger.LogInformation("{CommandName} execution completed.", command.ContractName);
        activity.SetStatus(activityStatus);
        activity.Stop();
        
        return result;
    }
    
    protected async Task<bool> EnqueueDistributedMessage(IContract contract, CancellationToken cancellationToken,
        string sourceMethod, int sourceLineNumber, string sourceFileName)
    {
        var message = new DistributedMessageEvent(contract, this._correlation.ToDictionary());
        
        message.CorrelateTo(contract);
        message.PrepareToExecute();

        return await this.PublishAsync(message, cancellationToken);
    }
    
    /// <summary>
    /// Publishes validation failure notifications to the notification store.
    /// </summary>
    /// <param name="failures">
    /// The collection of validation failures retrieved 
    /// </param>
    private Task NotifyValidationFailuresAsync(IEnumerable<ValidationFailure> failures, CancellationToken cancellationToken, 
        string sourceMethod, int sourceLineNumber, string sourceFileName)
    {
        var tasks = failures.Select(failure =>
        {
            Notification notification = new (failure.ErrorCode, failure.ErrorMessage, sourceMethod, sourceLineNumber, sourceFileName);
            
            var traceId = this._correlation.GetTraceId();
            var correlationId = this._correlation.GetCorrelationId();
            var idempotencyKey = this._correlation.GetIdempotencyKey();
        
            notification.CorrelateTo(traceId, correlationId, idempotencyKey);
            
            return this.PublishAsync(notification, cancellationToken);
        });

        Task.WaitAll(tasks, cancellationToken);
        
        return Task.CompletedTask;
    }

    private async Task ExecuteOnSandboxAsync(IContract contract, Func<Task> action, string sourceMethod, 
        int sourceLineNumber, string sourceFileName)
    {
        await this.ExecuteOnSandboxAsync(
            contract, 
            async () =>
            {
                await action();
                return !this._store.HasNotifications();
            }, 
            sourceMethod, sourceLineNumber, sourceFileName);
    }
    
    private async Task<TResult> ExecuteOnSandboxAsync<TResult>(IContract contract, Func<Task<TResult>> action, string sourceMethod, 
        int sourceLineNumber, string sourceFileName)
    {
        var activityName = $"{contract.ContractName} Execution";
        
        using var activity = this._otel.CreateActivity(activityName, ActivityKind.Internal) ?? new Activity(activityName);
        activity.Start();
        
        try
        {
            var result = await action();

            var activityStatus = ActivityStatusCode.Ok;
            var activityMsg = $"{contract.ContractName} executed successfully.";
        
            if (contract is not Notification && this._store.HasNotifications())
            {
                activityStatus = ActivityStatusCode.Error;
                activityMsg = $"{contract.ContractName} execution failed.";
                this._logger.LogInformation("{logMessage}", activityMsg);
            }
            
            activity.SetStatus(activityStatus, activityMsg);
            activity.Stop();
            
            return result;
        }
        catch (Exception e)
        {
            var errorMsg = $"{contract.ContractName} occurred unexpected system error during execution.";
            
            SystemError error = new(errorMsg, e, sourceMethod, sourceLineNumber, sourceFileName);

            await this.PublishAsync(error, CancellationToken.None, sourceMethod, sourceLineNumber, sourceFileName);
            
            activity.SetStatus(ActivityStatusCode.Error, errorMsg);
            activity.Stop();
            
            return default(TResult);
        }
    }
}