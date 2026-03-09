using EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.AccountValidators;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator(IServiceProvider provider)
    {
        var accountRepository = provider.GetRequiredService<IAccountReadRepository>();
        
        RuleFor(r => r.Request)
            .NotNull();

        RuleFor(r => r.Request.Name)
            .NotEmpty()
                .WithErrorCode(AccountValidationErrors.RequiredField.ErrorCode)
                .WithMessage(AccountValidationErrors.RequiredField.ErrorMessage)
            .MinimumLength(5)
                .WithErrorCode(AccountValidationErrors.InvalidField.ErrorCode)
                .WithMessage(AccountValidationErrors.InvalidField.ErrorMessage);
        
        RuleFor(r => r.AccountId)
            .Must(accId => accountRepository.Exists(accId))
                .WithErrorCode(AccountValidationErrors.AccountNotExists.ErrorCode)
                .WithMessage(AccountValidationErrors.AccountNotExists.ErrorMessage);
    }
}