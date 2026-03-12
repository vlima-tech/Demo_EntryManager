using EntryManager.Core.Transaction.Api.Application.Commands.GroupCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.GroupValidators;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    private readonly IAccountReadRepository _accountRepository;
    private readonly IGroupReadRepository _groupRepository;
    
    public CreateGroupCommandValidator(IServiceProvider provider)
    {
        this._accountRepository = provider.GetRequiredService<IAccountReadRepository>();
        this._groupRepository = provider.GetRequiredService<IGroupReadRepository>();
        
        RuleFor(c => c.Request)
            .NotNull();
        
        this.ValidateRequestBody();
        this.ValidateAccount();
        this.ValidateGroup();
    }

    private void ValidateRequestBody()
    {
        RuleFor(c => c.Request.Name)
            .NotEmpty()
                .WithErrorCode(GroupValidationErrors.RequiredField.ErrorCode)
                .WithMessage(GroupValidationErrors.RequiredField.ErrorMessage)
            .MinimumLength(5)
                .WithErrorCode(GroupValidationErrors.InvalidField.ErrorCode)
                .WithMessage(GroupValidationErrors.InvalidField.ErrorMessage)
            .Must(groupName => this._groupRepository.NotExists(groupName))
                .WithErrorCode(GroupValidationErrors.GroupAlreadyExists.ErrorCode)
                .WithMessage(GroupValidationErrors.GroupAlreadyExists.ErrorMessage);

        RuleFor(c => c.Request.AccountId)
            .NotEmpty()
                .WithErrorCode(GroupValidationErrors.RequiredField.ErrorCode)
                .WithMessage(GroupValidationErrors.RequiredField.ErrorMessage);

        RuleFor(c => c.Request.Type)
            .IsInEnum();
    }
    
    private void ValidateAccount()
    {
        RuleFor(c => c.Request.AccountId)
            .Must(accId => this._accountRepository.Exists(accId))
                .WithErrorCode(GroupValidationErrors.AccountNotExists.ErrorCode)
                .WithMessage(GroupValidationErrors.AccountNotExists.ErrorMessage);
    }
    
    private void ValidateGroup()
    {
        RuleFor(c => c.Request.AccountId)
            .Must(accId => this._groupRepository.NotExists(accId))
                .WithErrorCode(GroupValidationErrors.GroupAlreadyExists.ErrorCode)
                .WithMessage(GroupValidationErrors.GroupAlreadyExists.ErrorMessage);
    }
}