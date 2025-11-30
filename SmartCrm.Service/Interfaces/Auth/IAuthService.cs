using SmartCrm.Domain.Entities.Users;
using SmartCrm.Service.DTOs.Auth;
using SmartCrm.Service.DTOs.Users;

namespace SmartCrm.Service.Interfaces.Auth;

public interface IAuthService
{
    Task<UserDto> RegisterAdministratorAsync(AddUserDto addUserDto);


    Task<string> LoginAsync(LoginDto loginDto);
}
