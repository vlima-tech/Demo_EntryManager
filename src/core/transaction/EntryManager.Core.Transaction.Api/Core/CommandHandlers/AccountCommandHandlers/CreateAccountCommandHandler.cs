using EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;
using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Contracts.Events.AccountEvents;
using EntryManager.Core.Transaction.Contracts.Responses.AccountResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.AccountCommandHandlers;

public class CreateAccountCommandHandler(IServiceProvider provider) : ICommandHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    private readonly IServiceBus _serviceBus = provider.GetRequiredService<IServiceBus>();
    private IAccountRepository _accountRepository = provider.GetRequiredService<IAccountRepository>();
    
    public async Task<CreateAccountResponse?> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var account = new AccountModel(request.Name, request.Balance, AccountStatus.Active);
        
        await this._accountRepository.CreateAsync(account, cancellationToken);

        if (this._store.HasNotifications())
            return default;

        var @event = new AccountWasCreatedEvent(account.Id, account.Name, account.Balance, (Contracts.Enums.AccountStatus)account.Status);
        
        await this._serviceBus.PublishAsync(@event, cancellationToken);
        
        return new CreateAccountResponse
        {
            AccountId = account.Id,
            Name = account.Name,
            Status = (Contracts.Enums.AccountStatus)account.Status
        };
    }

    public void Dispose()
    {
        this._accountRepository?.Dispose();
        this._accountRepository = null;
    }
}