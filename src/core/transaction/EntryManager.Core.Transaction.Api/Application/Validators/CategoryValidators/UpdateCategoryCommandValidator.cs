using EntryManager.Core.Transaction.Api.Application.Commands.CategoryCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.CategoryValidators;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator(IServiceProvider provider)
    {
        var categoryRepository = provider.GetRequiredService<ICategoryReadRepository>();
        
        RuleFor(r => r.Request)
            .NotNull();

        RuleFor(r => r.Request.Title)
            .NotEmpty()
                .WithErrorCode(CategoryValidationErrors.RequiredField.ErrorCode)
                .WithMessage(CategoryValidationErrors.RequiredField.ErrorMessage)
            .MinimumLength(5)
                .WithErrorCode(CategoryValidationErrors.InvalidField.ErrorCode)
                .WithMessage(CategoryValidationErrors.InvalidField.ErrorMessage);
        
        RuleFor(r => r.CategoryId)
            .Must(categoryId => categoryRepository.Exists(categoryId))
                .WithErrorCode(CategoryValidationErrors.CategoryNotExists.ErrorCode)
                .WithMessage(CategoryValidationErrors.CategoryNotExists.ErrorMessage);
    }
}