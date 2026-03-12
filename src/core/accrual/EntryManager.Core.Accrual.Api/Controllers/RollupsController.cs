using EntryManager.Core.Accrual.Api.Application.Commands.RollupCommands;
using EntryManager.Core.Accrual.Api.Domain.Interfaces;
using EntryManager.Shared.Api.Response;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Core.Accrual.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RollupsController(IServiceProvider provider) : InternalControllerBase(provider)
{
    private IRollupRepository _rollupRepository = provider.GetRequiredService<IRollupRepository>();
    
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        this.PropagateContext();
        
        var result = await this._rollupRepository.LoadAsync(DateTime.UtcNow, cancellationToken);
        
        return await this.ResponseAsync(result);
    }
    
    [HttpPut("{rollupDay:datetime}/reset")]
    public async Task<IActionResult> ResetAsync(DateTime rollupDay, CancellationToken cancellationToken)
    {
        this.PropagateContext();

        await this.ServiceBus.SendAsync(new ResetRollupCommand(rollupDay), cancellationToken);
        var result = await this._rollupRepository.LoadAsync(rollupDay, cancellationToken);
        
        return await this.ResponseAsync(result);
    }
}