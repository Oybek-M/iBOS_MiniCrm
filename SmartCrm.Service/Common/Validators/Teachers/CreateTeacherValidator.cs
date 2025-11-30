using FluentValidation;
using SmartCrm.Service.DTOs.Teachers;

namespace SmartCrm.Service.Common.Validators.Teachers;

public class CreateTeacherValidator : AbstractValidator<CreateTeacherDto>
{
    public CreateTeacherValidator()
    {
        RuleFor(dto => dto.FirstName).NotEmpty().WithMessage("Ism kiritilishi shart.");
        RuleFor(dto => dto.LastName).NotEmpty().WithMessage("Familiya kiritilishi shart.");

        RuleFor(dto => dto.PhoneNumber)
            .NotEmpty()
            .WithMessage("Telefon raqam kiritilishi shart.")
            .Matches(@"^\+998[0-9]{9}$")
            .WithMessage("Telefon raqam noto'g'ri formatda. Masalan: +998991234567");

        RuleFor(dto => dto.Password).NotEmpty().WithMessage("Parol kiritilishi shart.");
    }
}
