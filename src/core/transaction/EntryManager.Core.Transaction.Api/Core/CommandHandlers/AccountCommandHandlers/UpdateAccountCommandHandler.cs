using System.Runtime.CompilerServices;
using EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.AccountCommandHandlers;

public class UpdateAccountCommandHandler(IServiceProvider provider) : ICommandHandler<UpdateAccountCommand>
{
    private IAccountRepository _accountRepository = provider.GetRequiredService<IAccountRepository>();
    
    public async Task<bool> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        var account = await this._accountRepository.FindByIdAsync(command.AccountId, cancellationToken);
        
        account.ChangeName(request.Name);
        
        return await this._accountRepository.UpdateAsync(account, cancellationToken);
    }

    public void Dispose()
    {
        
    }
}