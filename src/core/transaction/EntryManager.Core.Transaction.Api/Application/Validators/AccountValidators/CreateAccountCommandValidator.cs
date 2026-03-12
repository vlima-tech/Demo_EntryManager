using EntryManager.Core.Transaction.Api.Application.Commands.AccountCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.AccountValidators;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator(IServiceProvider provider)
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
                .WithMessage(AccountValidationErrors.InvalidField.ErrorMessage)
            .Must(accName => accountRepository.NotExists(accName))
                .WithErrorCode(AccountValidationErrors.AccountAlreadyExists.ErrorCode)
                .WithMessage(AccountValidationErrors.AccountAlreadyExists.ErrorMessage);
    }
}