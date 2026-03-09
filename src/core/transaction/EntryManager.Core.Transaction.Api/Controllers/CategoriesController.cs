using EntryManager.Core.Transaction.Api.Application.Commands.CategoryCommands;
using EntryManager.Core.Transaction.Api.Application.Queries;
using EntryManager.Core.Transaction.Contracts.Requests.CategoryRequests;
using EntryManager.Shared.Api.Response;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Core.Transaction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(IServiceProvider provider) : InternalControllerBase(provider)
{
    private readonly ICategoryQuery _query = provider.GetRequiredService<ICategoryQuery>();
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        base.PropagateContext();
        
        var result = await this.ServiceBus.SendAsync(new CreateCategoryCommand(request), cancellationToken);

        return await base.ResponseAsync(result);
    }
    
    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        base.PropagateContext();
        
        var result = await this.ServiceBus.SendAsync(new UpdateCategoryCommand(categoryId, request), cancellationToken);

        return await base.ResponseAsync(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken)
    {
        var result = await this._query.ObtainsAllAsync(cancellationToken);
        
        return await this.ResponseAsync(result);
    }
}