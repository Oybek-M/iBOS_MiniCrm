using SmartCrm.Service.DTOs.Teachers;

namespace SmartCrm.Service.Interfaces.Teachers;

public interface ITeacherService
{
    Task<TeacherDto> CreateAsync(CreateTeacherDto dto);
    Task<TeacherDto> GetByIdAsync(Guid id);
    Task<IEnumerable<TeacherDto>> GetAllAsync();
    Task<TeacherDto> UpdateAsync(Guid id, UpdateTeacherDto dto);
    Task<bool> DeleteAsync(Guid id);
}
