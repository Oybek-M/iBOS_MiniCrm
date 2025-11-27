using FluentValidation;
using SmartCrm.Service.DTOs.Groups;

namespace SmartCrm.Service.Common.Validators.Groups;

public class CreateGroupValidator : AbstractValidator<CreateGroupDto>
{
    public CreateGroupValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("Guruh nomi kiritilishi shart.")
            .MinimumLength(3).WithMessage("Guruh nomi kamida 3 belgidan iborat bo'lishi kerak.");

        RuleFor(dto => dto.TeacherId)
            .NotEmpty().WithMessage("O'qituvchi tanlanishi shart.");
    }
}
