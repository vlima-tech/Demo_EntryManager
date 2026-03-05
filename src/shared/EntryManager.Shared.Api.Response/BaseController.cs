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
    public readonly IServiceProvider Provider = provider;
    public readonly IServiceBus ServiceBus = provider.GetService<IServiceBus>();
    public readonly INotificationStore Store = provider.GetService<INotificationStore>();
    public readonly IConfiguration Configuration = provider.GetService<IConfiguration>();
    public readonly ILogger Logger = provider.GetService<ILogger>();
    public readonly IHostEnvironment Env = provider.GetService<IHostEnvironment>();
    public readonly ICorrelation _correlation = provider.GetService<ICorrelation>();

    public void PropagateContext(IHeaderDictionary headers)
        => this.PropagateContext(headers.ToDictionary());
    
    public void PropagateContext(IDictionary<string, StringValues> headers)
        => this._correlation.PropagateContext(headers);
    
    protected virtual bool ResultIsValid() => !this.Store.HasNotifications();
    
    protected virtual bool ResultIsValid(bool includeNotifications = true, bool includeWarnings = false, bool includeSystemErrors = true)
        => !this.Store.HasNotifications(includeNotifications, includeWarnings, includeSystemErrors);
    
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

    protected abstract Task<IActionResult> ResponseAsync(HttpStatusCode statusCode, object? response = null);
}