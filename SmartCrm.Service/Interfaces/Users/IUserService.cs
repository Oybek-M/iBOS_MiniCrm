using SmartCrm.Service.DTOs.Users;

namespace SmartCrm.Service.Interfaces.Users;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);

    Task<IEnumerable<UserDto>> GetAllAdminsAsync();
    Task<IEnumerable<UserDto>> GetAllTeachersAsync();

    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeleteAsync(Guid id);

    Task<UserDto> ChangeStatusAsync(Guid id, bool isActive);

    Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto);

    Task<int> GetTotalStudentCountAsync();
}