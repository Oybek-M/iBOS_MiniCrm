using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCrm.Domain.Enums;
using SmartCrm.Service.DTOs.Users;
using SmartCrm.Service.Interfaces.Users;
using System.Security.Claims;

namespace SmartCrm.Controllers.Common
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "SuperAdministrator")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Roles = "SuperAdministrator, Administrator, Teacher")]
        [HttpGet("role")]
        public IActionResult GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (roleClaim is null)
            {
                _logger.LogWarning("Foydalanuvchi tokenida 'Role' claim'i topilmadi. User ID: {UserId}", User.FindFirstValue("Id"));
                return Unauthorized(new { message = "Foydalanuvchi roli aniqlanmadi." });
            }

            _logger.LogInformation("Foydalanuvchi roli so'raldi: {Role}", roleClaim.Value);

            if (Enum.TryParse<Role>(roleClaim.Value, ignoreCase: true, out var role))
            {
                return Ok(new { Role = role.ToString() });
            }
            else
            {
                _logger.LogError("Token'dagi rol ({ClaimRole}) 'Role' enum'iga mos kelmadi.", roleClaim.Value);
                return BadRequest(new { message = "Token'dagi rol formati noto'g'ri." });
            }
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdminsAsync()
        {
            var admins = await _userService.GetAllAdminsAsync();
            return Ok(admins);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updatedUser = await _userService.UpdateAsync(id, dto);
            return Ok(updatedUser);
        }

        [Authorize(Roles = "SuperAdministrator")]
        [HttpPatch("{id}/change-password")]
        public async Task<IActionResult> ChangePasswordAsync(Guid id, [FromBody] ChangePasswordDto dto)
        {
            await _userService.ChangePasswordAsync(id, dto);
            return Ok(new { message = "Parol muvaffaqiyatli o'zgartirildi." });
        }

        [Authorize(Roles = "SuperAdministrator")]
        [HttpPatch("{id}/change-status")]
        public async Task<IActionResult> ChangeStatusAsync(Guid id, [FromQuery] bool isActive)
        {
            var updatedUser = await _userService.ChangeStatusAsync(id, isActive);
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}