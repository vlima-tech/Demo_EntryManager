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
}

public struct InternalApiResponse
{
    public object? Data { get; set; }

    public IEnumerable<Warning> Warnings { get; set; }
    
    public IEnumerable<Notification> Errors { get; set; }

    public IEnumerable<SystemError> SystemErrors { get; set; }

    public IEnumerable<Log> Logs { get; set; }
}