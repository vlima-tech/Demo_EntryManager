using FluentValidation.Results;

namespace EntryManager.Core.Transaction.Api.Application.Validators.CategoryValidators;

public class CategoryValidationErrors
{
    public static readonly ValidationFailure RequiredField = new () { ErrorCode = "VALIDATION_FAILURE", ErrorMessage = "The field '{PropertyName}' is required and cannot be empty."};
    public static readonly ValidationFailure InvalidField = new () { ErrorCode = "VALIDATION_FAILURE", ErrorMessage = "The field '{PropertyName}' must have at least {MinLength} characters."};
    public static readonly ValidationFailure CategoryNotExists = new () { ErrorCode = "CATEGORY_NOT_FOUND", ErrorMessage = "The '{PropertyName}' was not found with the provided information."};
    public static readonly ValidationFailure GroupNotExists = new () { ErrorCode = "GROUP_NOT_FOUND", ErrorMessage = "The '{PropertyName}' was not found with the provided information."};
}