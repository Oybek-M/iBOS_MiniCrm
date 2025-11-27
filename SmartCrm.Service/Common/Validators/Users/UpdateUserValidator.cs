using FluentValidation;
using SmartCrm.Service.DTOs.Users;

namespace SmartCrm.Service.Common.Validators.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(dto => dto.Username).NotEmpty().MinimumLength(4)
            .WithMessage("Username kamida 4 belgidan iborat bo'lishi kerak.");
    }
}
