using FluentValidation;
using SmartCrm.Service.DTOs.Attendances;

namespace SmartCrm.Service.Common.Validators.Attendances
{
    public class CreateAttendanceValidator : AbstractValidator<CreateAttendanceDto>
    {
        public CreateAttendanceValidator()
        {
            RuleFor(dto => dto.StudentId).NotEmpty().WithMessage("O'quvchi tanlanishi shart");
            RuleFor(dto => dto.Status).NotEmpty().WithMessage("Status belgilanishi shart");
        }
    }
}
