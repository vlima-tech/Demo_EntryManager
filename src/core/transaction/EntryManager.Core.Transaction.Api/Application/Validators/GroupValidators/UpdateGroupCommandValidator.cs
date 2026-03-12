using EntryManager.Core.Transaction.Api.Application.Commands.GroupCommands;
using EntryManager.Core.Transaction.Api.Domain.Interfaces.Repositories;
using FluentValidation;

namespace EntryManager.Core.Transaction.Api.Application.Validators.GroupValidators;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator(IServiceProvider provider)
    {
        var groupRepository = provider.GetRequiredService<IGroupReadRepository>();
        
        RuleFor(r => r.Request)
            .NotNull();

        RuleFor(r => r.Request.Name)
            .NotEmpty()
                .WithErrorCode(GroupValidationErrors.RequiredField.ErrorCode)
                .WithMessage(GroupValidationErrors.RequiredField.ErrorMessage)
            .MinimumLength(5)
                .WithErrorCode(GroupValidationErrors.InvalidField.ErrorCode)
                .WithMessage(GroupValidationErrors.InvalidField.ErrorMessage);
        
        RuleFor(r => r.GroupId)
            .Must(groupId => groupRepository.Exists(groupId))
                .WithErrorCode(GroupValidationErrors.GroupNotExists.ErrorCode)
                .WithMessage(GroupValidationErrors.GroupNotExists.ErrorMessage);
    }
}