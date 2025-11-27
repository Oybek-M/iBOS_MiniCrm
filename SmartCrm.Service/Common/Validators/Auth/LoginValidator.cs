using FluentValidation;
using SmartCrm.Service.DTOs.Auth;

namespace SmartCrm.Service.Common.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(dto => dto.Username).NotEmpty().WithMessage("Username kiritilishi shart.");
        RuleFor(dto => dto.Password).NotEmpty().WithMessage("Parol kiritilishi shart.");
    }
}