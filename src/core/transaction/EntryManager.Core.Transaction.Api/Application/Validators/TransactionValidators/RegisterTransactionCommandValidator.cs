using EntryManager.Core.Transaction.Api.Application.Commands.TransactionCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.TransactionValidators;

public class RegisterTransactionCommandValidator : AbstractValidator<RegisterTransactionCommand>
{
    public RegisterTransactionCommandValidator(IServiceProvider provider)
    {
        var categoryRepository = provider.GetRequiredService<ICategoryReadRepository>();
        var transactionRepository = provider.GetRequiredService<ITransactionReadRepository>();
        
        RuleFor(r => r.Request)
            .NotNull();

        RuleFor(r => r.IdempotencyKey)
            .Must(idempotencyKey => transactionRepository.IsUniqueKey(idempotencyKey))
            .WithErrorCode(TransactionValidationErrors.IdempotencyKeyConflict.ErrorCode)
            .WithMessage(TransactionValidationErrors.IdempotencyKeyConflict.ErrorMessage);
        
        RuleFor(r => r.Request.Title)
            .NotEmpty()
                .WithErrorCode(TransactionValidationErrors.RequiredField.ErrorCode)
                .WithMessage(TransactionValidationErrors.RequiredField.ErrorMessage)
            .MinimumLength(5)
                .WithErrorCode(TransactionValidationErrors.InvalidField.ErrorCode)
                .WithMessage(TransactionValidationErrors.InvalidField.ErrorMessage);
        
        RuleFor(r => r.Request.Value)
            .GreaterThanOrEqualTo(0)
                .WithErrorCode(TransactionValidationErrors.ValueMustBePositive.ErrorCode)
                .WithMessage(TransactionValidationErrors.ValueMustBePositive.ErrorMessage);
        
        RuleFor(r => r.Request.Date)
            .LessThanOrEqualTo(DateTime.UtcNow)
                .WithErrorCode(TransactionValidationErrors.DateMustBeInPast.ErrorCode)
                .WithMessage(TransactionValidationErrors.DateMustBeInPast.ErrorMessage)
            .When(r => r.Request.Date.HasValue);
        
        RuleFor(r => r.Request.CategoryId)
            .Must(groupId => categoryRepository.Exists(groupId))
                .WithErrorCode(TransactionValidationErrors.CategoryNotExists.ErrorCode)
                .WithMessage(TransactionValidationErrors.CategoryNotExists.ErrorMessage);
    }
}