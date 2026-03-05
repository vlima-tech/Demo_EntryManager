using System.Net;
using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;
using EntryManager.Core.Transaction.Contracts.Requests.TransactionRequests;
using EntryManager.Shared.Api.Response;
using Microsoft.AspNetCore.Mvc;

namespace EntryManager.Core.Transaction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(IServiceProvider provider) : InternalControllerBase(provider)
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        this.PropagateContext(this.Request.Headers);
        
        var cmd = new CreateTransactionCommand(new CreateTransactionRequest{ Message = "Hello World, Transaction Command!" });
        await this.ServiceBus.SendAsync(cmd, cancellationToken);
        
 
        var @event = new TransactionWasCreatedEvent(new CreateTransactionRequest{ Message = "Hello World, Transaction Event!" });
        await this.ServiceBus.PublishAsync(@event, cancellationToken);
    
        return await base.ResponseAsync(HttpStatusCode.Accepted);
    }
}