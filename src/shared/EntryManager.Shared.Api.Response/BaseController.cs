using System.Diagnostics;
using System.Net;
using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Interop;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace EntryManager.Shared.Api.Response;

public abstract class BaseController(IServiceProvider provider) : ControllerBase
{
    protected readonly IServiceBus ServiceBus = provider.GetRequiredService<IServiceBus>();
    protected readonly INotificationStore Store = provider.GetRequiredService<INotificationStore>();
    protected readonly IConfiguration Configuration = provider.GetRequiredService<IConfiguration>();
    protected readonly ILogger Logger = provider.GetRequiredService<ILogger<BaseController>>();
    protected readonly IHostEnvironment Env = provider.GetRequiredService<IHostEnvironment>();
    private readonly ICorrelation _correlation = provider.GetRequiredService<ICorrelation>();
    
    /// <summary>
    /// Automatically propagates correlation keys (traceId, traceparent, correlationId, idempotencyKey) 
    /// from the current HTTP request headers.
    /// </summary>
    protected virtual void PropagateContext()
    {
        var headers = this.Request.Headers;
        Dictionary<string, StringValues> correlationKeys = new();

        var traceIdKey = headers.TryGetValue(KnowHeaders.TraceId, out var traceId)
            ? traceId.ToString()
            : Activity.Current?.TraceId.ToHexString() ?? new Activity("CorrelationContext").TraceId.ToHexString();
        
        var traceKey = headers.TryGetValue(KnowHeaders.Trace, out var trace)
            ? trace.ToString()
            : Activity.Current?.Id ?? new Activity("CorrelationContext").Id;
        
        var correlationIdKey = headers.TryGetValue(KnowHeaders.CorrelationId, out var correlationId)
            ? correlationId.ToString()
            : Guid.NewGuid().ToString();
        
        var idempotencyKey = headers.TryGetValue(KnowHeaders.IdempotencyKey, out var idempotency)
            ? idempotency.ToString()
            : Guid.NewGuid().ToString();

        var metadataKey = headers.TryGetValue(KnowHeaders.Metadata, out var metadata)
            ? metadata.ToString()
            : string.Empty;
        
        correlationKeys.Add(KnowHeaders.TraceId, traceIdKey);
        correlationKeys.Add(KnowHeaders.Trace, traceKey);
        correlationKeys.Add(KnowHeaders.CorrelationId, correlationIdKey);
        correlationKeys.Add(KnowHeaders.IdempotencyKey, idempotencyKey);
        correlationKeys.Add(KnowHeaders.Metadata, metadataKey);
        
        this.PropagateContext(correlationKeys);
    }
    
    /// <summary>
    /// Propagates correlation context using the provided <see cref="IHeaderDictionary"/>.
    /// </summary>
    /// <param name="headers">The collection of HTTP headers to extract correlation keys from.</param>
    protected virtual void PropagateContext(IHeaderDictionary headers)
        => this.PropagateContext(headers.ToDictionary());
    
    protected virtual void PropagateContext(IDictionary<string, StringValues> headers)
        => this._correlation.PropagateContext(headers);
    
    /// <summary>
    /// Checks if the current operation result is valid based on the absence of notifications.
    /// </summary>
    /// <returns>True if no notifications exist; otherwise, false.</returns>
    protected virtual bool ResultIsValid() => !this.Store.HasNotifications();
    
    /// <summary>
    /// Checks if the current operation result is valid based on specific notification categories.
    /// </summary>
    /// <param name="includeNotifications">Whether to check for business notifications.</param>
    /// <param name="includeWarnings">Whether to check for warning messages.</param>
    /// <param name="includeSystemErrors">Whether to check for critical system errors.</param>
    /// <returns>True if no notifications of the specified types exist; otherwise, false.</returns>
    protected virtual bool ResultIsValid(bool includeNotifications = true, bool includeWarnings = false, bool includeSystemErrors = true)
        => !this.Store.HasNotifications(includeNotifications, includeWarnings, includeSystemErrors);
    
    /// <summary>
    /// Publishes all <see cref="ModelState"/> errors as notifications and returns a Bad Request response.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing the <see cref="IActionResult"/>.</returns>
    protected virtual Task<IActionResult> NotifyModelStateErrorsAsync(CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();
        var errors = ModelState.Values.SelectMany(v => v.Errors);

        foreach (var erro in errors)
        {
            var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
            var notification = new Notification(code: "VALIDATION_ERROR", errorMsg);
            
            var notificationTask = this.ServiceBus.PublishAsync(notification, cancellationToken);
            
            tasks.Add(notificationTask);
        }
        
        Task.WaitAll(tasks.ToArray(), cancellationToken);

        return ResponseAsync(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// When implemented in a derived class, generates a standardized asynchronous response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code for the response.</param>
    /// <param name="response">The optional data to be returned in the response body.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing the <see cref="IActionResult"/>.</returns>
    protected abstract Task<IActionResult> ResponseAsync(HttpStatusCode statusCode, object? response = null);
}