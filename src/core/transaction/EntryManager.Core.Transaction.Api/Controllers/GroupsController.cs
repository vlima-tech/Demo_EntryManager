using System.Net;
using EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;
using EntryManager.Core.Transaction.Api.Application.Commands.GroupCommands;
using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Contracts.Requests.AccountRequests;
using EntryManager.Core.Transaction.Contracts.Requests.GroupRequests;
using EntryManager.Shared.Api.Response;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Core.Transaction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController(IServiceProvider provider) : InternalControllerBase(provider)
{
    private readonly IGroupQuery _query = provider.GetRequiredService<IGroupQuery>();
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await this.ServiceBus.SendAsync(new CreateGroupCommand(request), cancellationToken);

        return await base.ResponseAsync(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken)
    {
        var result = await this._query.ObtainsAllAsync(cancellationToken);
        
        return await this.ResponseAsync(result);
    }
    
    [HttpGet("{groupId:guid}/details")]
    public async Task<IActionResult> FindByIdAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var result = await this._query.ObtainsByIdAsync(groupId, cancellationToken);
        
        var statusCode = result is null ? HttpStatusCode.NotFound : HttpStatusCode.OK;
        
        return await this.ResponseAsync(statusCode, result);
    }
    
    [HttpPut("{groupId:guid}")]
    public async Task<IActionResult> ChangeAsync(Guid groupId, [FromBody] UpdateGroupRequest request, CancellationToken cancellationToken)
    {
        this.PropagateContext();
        
        var result = await this.ServiceBus.SendAsync(new UpdateGroupCommand(groupId, request), cancellationToken);
        
        return await this.ResponseAsync(result);
    }
}