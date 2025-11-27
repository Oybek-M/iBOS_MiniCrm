using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Enums;

namespace SmartCrm.Data.Interfaces.Students;

public interface IStudent : IRepository<Student>
{
    Task<IEnumerable<Student>> GetStudentsByPaymentStatusAsync(MonthlyPaymentStatus status);
}
