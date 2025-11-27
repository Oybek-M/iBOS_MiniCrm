using FluentValidation;
using SmartCrm.Service.DTOs.Payments;

namespace SmartCrm.Service.Common.Validators.Payments;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(dto => dto.StudentId).NotEmpty().WithMessage("O'quvchi tanlanishi shart.");

        RuleFor(dto => dto.AmountPaid)
            .GreaterThan(0).WithMessage("To'lov summasi 0 dan katta bo'lishi kerak.");

        RuleFor(dto => dto.Type).IsInEnum().WithMessage("To'lov turi noto'g'ri kiritildi.");
    }
}
