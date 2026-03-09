using EntryManager.Core.Transaction.Api.Application.Commands.CategoryCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.CategoryValidators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(IServiceProvider provider)
    {
        var groupRepository = provider.GetRequiredService<IGroupReadRepository>();
        
        RuleFor(r => r.Request)
            .NotNull();

        RuleFor(r => r.Request.Name)
            .NotEmpty()
                .WithErrorCode(CategoryValidationErrors.RequiredField.ErrorCode)
                .WithMessage(CategoryValidationErrors.RequiredField.ErrorMessage)
            .MinimumLength(5)
                .WithErrorCode(CategoryValidationErrors.InvalidField.ErrorCode)
                .WithMessage(CategoryValidationErrors.InvalidField.ErrorMessage);
        
        RuleFor(r => r.Request.GroupId)
            .NotEmpty()
                .WithErrorCode(CategoryValidationErrors.RequiredField.ErrorCode)
                .WithMessage(CategoryValidationErrors.RequiredField.ErrorMessage)
            .Must(accId => groupRepository.Exists(accId))
                .WithErrorCode(CategoryValidationErrors.GroupNotExists.ErrorCode)
                .WithMessage(CategoryValidationErrors.GroupNotExists.ErrorMessage);
    }
}