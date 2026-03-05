using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.TransactionCommandHandlers;

public class CreateTransactionCommandHandler(IServiceProvider provider) : ICommandHandler<CreateTransactionCommand>
{
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public Task<bool> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public void Dispose()
    {
        
    }
}

public class TransactionWasCreatedEventHandler(IServiceProvider provider) : IEventHandler<TransactionWasCreatedEvent>
{
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public Task Handle(TransactionWasCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        
    }
}

public class TransactionWasCreatedEventHandler2(IServiceProvider provider) : IEventHandler<TransactionWasCreatedEvent>
{
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    
    public Task Handle(TransactionWasCreatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        
    }
}