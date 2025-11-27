using FluentValidation;
using SmartCrm.Service.DTOs.Users;

namespace SmartCrm.Service.Common.Validators.Users;

public class AddUserValidator : AbstractValidator<AddUserDto>
{
    public AddUserValidator()
    {
        RuleFor(dto => dto.Username).NotEmpty().MinimumLength(4)
            .WithMessage("Username kamida 4 belgidan iborat bo'lishi kerak.");

        RuleFor(dto => dto.Password).NotEmpty().MinimumLength(6)
            .WithMessage("Parol kamida 6 belgidan iborat bo'lishi kerak.");
    }
}
