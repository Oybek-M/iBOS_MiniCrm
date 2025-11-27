using FluentValidation;
using SmartCrm.Service.DTOs.Students;

namespace SmartCrm.Service.Common.Validators.Students;

public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentValidator()
    {
        RuleFor(dto => dto.FirstName).NotEmpty().WithMessage("Ism kiritilishi shart.");
        RuleFor(dto => dto.LastName).NotEmpty().WithMessage("Familiya kiritilishi shart.");

        RuleFor(dto => dto.PhoneNumber)
            .NotEmpty().WithMessage("Telefon raqam kiritilishi shart.")
            .Matches(@"^\+998[0-9]{9}$").WithMessage("Telefon raqam noto'g'ri formatda. Masalan: +998991234567");

        RuleFor(dto => dto.MonthlyPaymentAmount)
            .GreaterThan(0).WithMessage("Oylik to'lov 0 dan katta bo'lishi kerak.");

        RuleFor(dto => dto.DiscountPercentage)
            .InclusiveBetween(0, 100).WithMessage("Chegirma 0 va 100 orasida bo'lishi kerak.");

        RuleFor(dto => dto.GroupId).NotEmpty().WithMessage("Guruh tanlanishi shart.");
    }
}
