using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.TransactionValidators;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        /*
        RuleFor(r => r.Request)
            .NotNull();*/
    }
}