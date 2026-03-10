using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Contracts.Requests.TransactionRequests;
using EntryManager.Shared.Api.Response;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Core.Transaction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(IServiceProvider provider) : InternalControllerBase(provider)
{
    private readonly IAccountRepository _accountRepository = provider.GetRequiredService<IAccountRepository>();
    
    [HttpPost]
    public async Task<IActionResult> RegisterTransactionAsync([FromBody] RegisterTransactionRequest request, CancellationToken cancellationToken)
    {
        base.PropagateContext();
        
        var result = await this.ServiceBus.SendAsync(new RegisterTransactionCommand(request), cancellationToken);
        
        return await base.ResponseAsync(result);
    }
}