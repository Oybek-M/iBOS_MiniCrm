using SmartCrm.Domain.Enums;
using SmartCrm.Service.DTOs.Dashboard;
using SmartCrm.Service.DTOs.Students;

namespace SmartCrm.Service.Interfaces.Students;

public interface IStudentService
{
    Task<StudentDto> CreateAsync(CreateStudentDto dto);
    Task<StudentDto> GetByIdAsync(Guid id);
    Task<IEnumerable<StudentDto>> GetByGroupIdAsync(Guid id);
    Task<IEnumerable<StudentDto>> GetAllAsync();
    Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto dto);
    Task<bool> DeleteAsync(Guid id);

    Task<PaymentStatusFilterResultDto> GetStudentsByPaymentStatusAsync(MonthlyPaymentStatus status);

    Task<IEnumerable<StudentDto>> SearchAsync(string searchTerm);
}
