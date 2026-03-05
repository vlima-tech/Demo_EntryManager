using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Interop;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace EntryManager.Shared.Bus.Kafka;

public class DistributedMessageEventConsumer(IServiceProvider provider) : IConsumer<DistributedMessageEvent>
{
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly ICorrelation _correlation = provider.GetRequiredService<ICorrelation>();
    
    public async Task Consume(ConsumeContext<DistributedMessageEvent> context)
    {
        var message = context.Message;
        
        this._correlation.PropagateContext(message);

        if (message.Data.ContractType == ContractType.Event)
        {
            var @event = message.Data as IEvent;
            
            @event.PrepareToExecute();
            await this._serviceBus.PublishAsync(@event, context.CancellationToken);
            
            return;
        }
        
        var command = message.Data as ICommand;
        
        command.PrepareToExecute();
        await this._serviceBus.SendAsync(command, context.CancellationToken);
    }
}