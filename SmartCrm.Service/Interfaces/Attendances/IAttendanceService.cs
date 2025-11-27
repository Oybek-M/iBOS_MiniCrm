using SmartCrm.Domain.Entities.Attendances;
using SmartCrm.Service.DTOs.Attendances;

namespace SmartCrm.Service.Interfaces.Attendances
{
    public interface IAttendanceService
    {
        Task<Attendance> CreateAsync(CreateAttendanceDto dto);
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<Attendance> GetByIdAsync(Guid id);
        Task<IEnumerable<Attendance>> GetAttendancesByGroupIdAsync(Guid groupId);
        Task<IEnumerable<Attendance>> GetAttendancesByStudentIdAsync(Guid studentId);
        Task<Attendance> UpdateAsync(Guid id, UpdateAttendanceDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
