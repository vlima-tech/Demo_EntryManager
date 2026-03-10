using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;
using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.TransactionCommandHandlers;

public class CreateTransactionCommandHandler(IServiceProvider provider) : ICommandHandler<RegisterTransactionCommand>
{
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    private ICategoryReadRepository _categoryRepository = provider.GetRequiredService<ICategoryReadRepository>();
    private ITransactionRepository _transactionRepository = provider.GetRequiredService<ITransactionRepository>();
    
    public async Task<bool> Handle(RegisterTransactionCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var idempotencyKey = command.IdempotencyKey;
        var registerDate = request.Date.GetValueOrDefault(DateTime.UtcNow);
        
        var category = await this._categoryRepository.FindByIdAsync(request.CategoryId, cancellationToken);
        
        var transaction = new TransactionModel(idempotencyKey, request.Title, request.Value, registerDate, category);
        
        await this._transactionRepository.CreateAsync(transaction, cancellationToken);

        if (this._store.HasNotifications())
            return false;

        var @event = new TransactionWasCreatedEvent
        {
            Transaction = new TransactionObject
            {
                TransactionId = transaction.Id,
                IdempotencyKey = transaction.IdempotencyKey,
                Title = transaction.Title,
                Value = transaction.Value,
                CreatedAt = transaction.CreatedAt,
                RegisteredAt = transaction.RegisteredAt,
                Category = new CategoryObject
                {
                    CategoryId = transaction.CategoryId,
                    Name = transaction.Category.Name,
                    Group = new GroupObject
                    {
                        GroupId = category.GroupId,
                        Name = category.Group.Name,
                        Type = (Contracts.Enums.EntryType)category.Group.Type,
                        Account = category.Group.Account.Name
                    }
                }
            }
        };
        
        await this._serviceBus.PublishAsync(@event, cancellationToken);
        
        return !this._store.HasNotifications();
    }

    public void Dispose()
    {
        this._categoryRepository?.Dispose();
        this._categoryRepository = null;
        
        this._transactionRepository?.Dispose();
        this._transactionRepository = null;
    }
}