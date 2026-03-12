using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Contracts.Events.TransactionEvents;
using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Core.Transaction.Contracts.Responses.TransactionResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.TransactionCommandHandlers;

public class CreateTransactionCommandHandler(IServiceProvider provider) : ICommandHandler<RegisterTransactionCommand, RegisterTransactionResponse>
{
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    private ICategoryReadRepository _categoryRepository = provider.GetRequiredService<ICategoryReadRepository>();
    private ITransactionRepository _transactionRepository = provider.GetRequiredService<ITransactionRepository>();
    
    public async Task<RegisterTransactionResponse?> Handle(RegisterTransactionCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var idempotencyKey = command.IdempotencyKey;
        var registerDate = request.Date.GetValueOrDefault(DateTime.UtcNow);
        
        var category = await this._categoryRepository.FindByIdAsync(request.CategoryId, cancellationToken);
        
        var transaction = new TransactionModel(idempotencyKey, request.Title, request.Value, registerDate, category);
        
        await this._transactionRepository.CreateAsync(transaction, cancellationToken);

        if (this._store.HasNotifications())
            return default;

        var @event = GenerateTransactionWasCreatedEvent(transaction);
        
        await this._serviceBus.PublishAsync(@event, cancellationToken);

        var response = GenerateResponse(transaction);

        return response;
    }

    private static TransactionWasCreatedEvent GenerateTransactionWasCreatedEvent(TransactionModel transaction)
    {
        var category = transaction.Category;
        var group = category.Group;
        var account = group.Account;
        
        var @event = new TransactionWasCreatedEvent
        {
            Transaction = new TransactionObject
            {
                TransactionId = transaction.Id,
                CategoryId = transaction.CategoryId,
                GroupId = category.GroupId,
                AccountId = category.Group.AccountId,
                IdempotencyKey = transaction.IdempotencyKey,
                Title = transaction.Title,
                Value = transaction.Value,
                CreatedAt = transaction.CreatedAt,
                OccurredAt = transaction.OccurredAt,
                Category = new CategoryObject
                {
                    CategoryId = category.Id,
                    Title = category.Title,
                    Group = new GroupObject
                    {
                        GroupId = group.Id,
                        Name = group.Name,
                        Description = group.Description,
                        Type = (Contracts.Enums.EntryType)group.Type,
                        Account = new AccountObject
                        {
                            AccountId = account.Id,
                            Name = account.Name,
                            Status = (Contracts.Enums.AccountStatus)account.Status
                        }
                    }
                }
            }
        };
        
        return @event;
    }

    private static RegisterTransactionResponse GenerateResponse(TransactionModel transaction)
    {
        var response = new RegisterTransactionResponse
        {
            TransactionId = transaction.Id,
            IdempotencyKey = transaction.IdempotencyKey,
            Title = transaction.Title,
            Value = transaction.Value,
            Category = new RegisterTransactionResponse.CategoryObject
            {
                CategoryId = transaction.CategoryId,
                Name = transaction.Category.Title,
                EntryType = (Contracts.Enums.EntryType)transaction.Category.EntryType,
                Group = transaction.Category.Group.Name
            },
            CreatedAt = transaction.CreatedAt
        };

        return response;
    }

    public void Dispose()
    {
        this._categoryRepository?.Dispose();
        this._categoryRepository = null;
        
        this._transactionRepository?.Dispose();
        this._transactionRepository = null;
    }
}