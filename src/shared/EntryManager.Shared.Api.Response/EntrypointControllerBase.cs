using System.Net;
using EntryManager.Shared.Bus.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Shared.Api.Response;

public class EntrypointControllerBase(IServiceProvider provider) : BaseController(provider)
{
    protected override Task<IActionResult> ResponseAsync(HttpStatusCode statusCode, object? result = null)
    {
        var response = new EntrypointApiResponse
        {
            Data =  result,
            Warnings = this.Store.GetWarnings().Select(item => new
            {
                item.Code,
                item.Message,
            }),
            Errors = this.Store.GetNotifications().Select(item => new
            {
                item.Code,
                item.Message,
            }),
        };
        
        var responseResult = new ObjectResult(response)
        {
            StatusCode = (int)statusCode
        };

        return Task.FromResult<IActionResult>(responseResult);
    }
}

public struct EntrypointApiResponse
{
    public object? Data { get; set; }

    public IEnumerable<dynamic> Warnings { get; set; }
    
    public IEnumerable<dynamic> Errors { get; set; }
}