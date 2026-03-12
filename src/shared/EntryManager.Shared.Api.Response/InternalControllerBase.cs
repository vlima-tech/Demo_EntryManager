using System.Net;
using EntryManager.Shared.Bus.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Shared.Api.Response;

public class InternalControllerBase(IServiceProvider provider) : BaseController(provider)
{
    protected override Task<IActionResult> ResponseAsync(HttpStatusCode statusCode, object? result = null)
    {
        var response = new InternalApiResponse
        {
            Data =  result,
            Warnings = this.Store.GetWarnings(),
            Errors = this.Store.GetNotifications(),
            SystemErrors = this.Store.GetSystemErrors(),
            Logs = this.Store.GetLogs()
        };
        
        var responseResult = new ObjectResult(response)
        {
            StatusCode = (int)statusCode
        };

        return Task.FromResult<IActionResult>(responseResult);
    }
    
    protected Task<IActionResult> ResponseAsync(object? result = null)
    {
        var response = new InternalApiResponse
        {
            Data =  result,
            Warnings = this.Store.GetWarnings(),
            Errors = this.Store.GetNotifications(),
            SystemErrors = this.Store.GetSystemErrors(),
            Logs = this.Store.GetLogs()
        };
        
        var responseResult = new ObjectResult(response)
        {
            StatusCode = (int)this.GetSuggestedStatusCode(result)
        };

        return Task.FromResult<IActionResult>(responseResult);
    }

    private HttpStatusCode GetSuggestedStatusCode(object? result = null)
    {
        if (!this.Store.HasNotifications() && result is null)
            return HttpStatusCode.NoContent;
        
        if (!this.Store.HasNotifications() && result is not null)
            return HttpStatusCode.OK;
        
        if (this.Store.HasSystemErrors())
            return HttpStatusCode.InternalServerError;

        // TODO: Add HasDomainErrors
        if (this.Store.HasNotifications())
            return HttpStatusCode.UnprocessableEntity;
        
        return HttpStatusCode.BadRequest;
    }
}

public struct InternalApiResponse
{
    public object? Data { get; set; }

    public IEnumerable<Warning> Warnings { get; set; }
    
    public IEnumerable<Notification> Errors { get; set; }

    public IEnumerable<SystemError> SystemErrors { get; set; }

    public IEnumerable<Log> Logs { get; set; }
}