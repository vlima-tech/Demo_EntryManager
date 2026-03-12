using FluentValidation.Results;

namespace EntryManager.Core.Transaction.Api.Application.Validators.TransactionValidators;

public class TransactionValidationErrors
{
    public static readonly ValidationFailure RequiredField = new () { ErrorCode = "VALIDATION_FAILURE", ErrorMessage = "The field '{PropertyName}' is required and cannot be empty."};
    public static readonly ValidationFailure InvalidField = new () { ErrorCode = "VALIDATION_FAILURE", ErrorMessage = "The field '{PropertyName}' must have at least {MinLength} characters."};
    public static readonly ValidationFailure CategoryNotExists = new () { ErrorCode = "CATEGORY_NOT_FOUND", ErrorMessage = "The '{PropertyName}' was not found with the provided information."};
    public static readonly ValidationFailure DateMustBeInPast = new () { ErrorCode = "VALIDATION_FAILURE", ErrorMessage = "The field '{PropertyName}' must be a date in the past."};
    public static readonly ValidationFailure ValueMustBePositive = new () { ErrorCode = "VALIDATION_FAILURE", ErrorMessage = "The field '{PropertyName}' must be positive. Negative conversions for expenses are handled automatically by the system."};
    public static readonly ValidationFailure IdempotencyKeyConflict = new () { ErrorCode = "IDEMPOTENCY_CONFLICT", ErrorMessage = "This transaction has already been processed."};   
}