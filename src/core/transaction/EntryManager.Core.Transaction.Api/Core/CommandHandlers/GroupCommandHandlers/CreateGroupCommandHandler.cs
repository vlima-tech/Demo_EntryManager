using EntryManager.Core.Transaction.Api.Application.Commands.GroupCommands;
using EntryManager.Core.Transaction.Api.Domain.Enums;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using EntryManager.Core.Transaction.Api.Domain.Models;
using EntryManager.Core.Transaction.Contracts.Objects;
using EntryManager.Core.Transaction.Contracts.Responses.GroupResponses;
using EntryManager.Shared.Bus.Abstractions;

namespace EntryManager.Core.Transaction.Api.Core.CommandHandlers.GroupCommandHandlers;

public class CreateGroupCommandHandler(IServiceProvider provider) : ICommandHandler<CreateGroupCommand, CreateGroupResponse?>
{
    private readonly INotificationStore _store = provider.GetRequiredService<INotificationStore>();
    private IGroupRepository _groupRepository = provider.GetRequiredService<IGroupRepository>();
    private IAccountReadRepository _accountRepository = provider.GetRequiredService<IAccountReadRepository>();
    
    public async Task<CreateGroupResponse?> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        var account = await this._accountRepository.FindByIdAsync(request.AccountId, cancellationToken);
        var group = GroupModel.Create(request.Name, request.Description, (EntryType)request.Type, account);
        
        await this._groupRepository.CreateAsync(group, cancellationToken);

        if (this._store.HasNotifications())
            return default;

        return new CreateGroupResponse
        {
            GroupId = group.Id,
            Name = group.Name,
            Type = (Contracts.Enums.EntryType)group.Type,
            Account = new AccountObject
            {
                AccountId = group.AccountId,
                Name = group.Account.Name,
                Status = (Contracts.Enums.AccountStatus)group.Account.Status
            }
        };
    }

    public void Dispose()
    {
        this._groupRepository = null;
        this._accountRepository = null;
    }
}