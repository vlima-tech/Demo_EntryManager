using FluentValidation;

namespace EntryManager.Shared.Bus.Abstractions.Validations;

internal class SystemErrorValidation : AbstractValidator<SystemError>
{
    public SystemErrorValidation() : base()
    {
        RuleFor(e => e.ContractId)
            .NotEqual(Guid.Empty)
            .WithMessage("The system error id is not defined.");

        RuleFor(e => e.Message)
            .NotEmpty()
            .WithMessage("The error message cannot be null or empty.");

        RuleFor(e => e.Exception)
            .NotEmpty()
            .WithMessage("The exception message cannot be null or empty.");

        RuleFor(e => e.StackTrace)
            .NotEmpty()
            .WithMessage("The stack trace cannot be null or empty.");

        RuleFor(e => e.SourceMethod)
            .NotEmpty()
            .WithMessage("The source method must be defined.");

        RuleFor(e => e.SourceLineNumber)
            .GreaterThan(0)
            .WithMessage("The source line number must be greater than zero.");

        RuleFor(e => e.SourceFileName)
            .NotEmpty()
            .WithMessage("The source file name must be defined.");
    }
}