using FluentValidation;
using SmartCrm.Service.DTOs.Users;

namespace SmartCrm.Service.Common.Validators.Users;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(dto => dto.NewPassword).NotEmpty().MinimumLength(6)
            .WithMessage("Yangi parol kamida 6 belgidan iborat bo'lishi kerak.");
    }
}
