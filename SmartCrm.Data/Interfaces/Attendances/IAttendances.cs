using SmartCrm.Domain.Entities.Attendances;

namespace SmartCrm.Data.Interfaces.Attendances
{
    public interface IAttendances : IRepository<Attendance>
    {
        Task<IEnumerable<Attendance>> GetAttendancesByGroup(Guid groupId);
        Task<IEnumerable<Attendance>> GetAttendancesByStudent(Guid studentId);
    }
}
