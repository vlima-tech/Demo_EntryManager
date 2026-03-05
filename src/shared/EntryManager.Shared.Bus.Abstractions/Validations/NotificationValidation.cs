using FluentValidation;

namespace EntryManager.Shared.Bus.Abstractions.Validations;

internal class NotificationValidation : AbstractValidator<Notification>
{
    public NotificationValidation()
    {
        RuleFor(n => n.ContractId)
            .NotEqual(Guid.Empty)
            .WithMessage("The notification id is not defined.");

        RuleFor(n => n.Message)
            .NotEmpty()
            .WithMessage("The message can not be null or empty.");
        
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