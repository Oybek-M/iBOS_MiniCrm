using FluentValidation;
using SmartCrm.Service.DTOs.Users;

namespace SmartCrm.Service.Common.Validators.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(dto => dto.FirstName).NotEmpty().WithMessage("Ism bo'lishi shart");

        RuleFor(dto => dto.LastName).NotEmpty().MinimumLength(3)
            .WithMessage("Ism kamida 3 belgidan iborat bo'lishi kerak.");

        RuleFor(dto => dto.Username).NotEmpty().MinimumLength(4)
            .WithMessage("Username kamida 4 belgidan iborat bo'lishi kerak.");
    }
}
