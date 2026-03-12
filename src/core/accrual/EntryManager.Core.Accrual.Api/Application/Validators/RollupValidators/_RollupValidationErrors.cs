using FluentValidation.Results;

namespace EntryManager.Core.Accrual.Api.Application.Validators.RollupValidators;

public class RollupValidationErrors
{
    public static readonly ValidationFailure RollupNotExists = new () { ErrorCode = "ROLLUP_NOT_FOUND", ErrorMessage = "The '{PropertyName}' was not found with the provided information."};
}