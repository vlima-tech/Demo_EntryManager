using System.Net;
using EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;
using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Contracts.Requests.AccountRequests;
using EntryManager.Shared.Api.Response;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Core.Transaction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IServiceProvider provider) : InternalControllerBase(provider)
{
    private readonly IAccountQuery _query = provider.GetRequiredService<IAccountQuery>();

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        base.PropagateContext();
        
        var result = await this.ServiceBus.SendAsync(new CreateAccountCommand(request), cancellationToken);
        
        return await this.ResponseAsync(result);
    }
    
    [HttpPut("{accountId:guid}")]
    public async Task<IActionResult> ChangeAsync(Guid accountId, [FromBody] UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        base.PropagateContext();
        
        var result = await this.ServiceBus.SendAsync(new UpdateAccountCommand(accountId, request), cancellationToken);
        
        return await this.ResponseAsync(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken)
    {
        var result = await this._query.ObtainsAllAsync(cancellationToken);
        
        return await this.ResponseAsync(result);
    }
    
    [HttpGet("{accountId:guid}/details")]
    public async Task<IActionResult> FindByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await this._query.FindByIdAsync(accountId, cancellationToken);
        
        var statusCode = result is null ? HttpStatusCode.NotFound : HttpStatusCode.OK;
        
        return await this.ResponseAsync(statusCode, result);
    }
}