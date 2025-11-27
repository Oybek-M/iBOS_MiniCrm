using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCrm.Service.DTOs.Auth;
using SmartCrm.Service.DTOs.Users;
using SmartCrm.Service.Interfaces.Auth;

namespace SmartCrm.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {

            var token = await _authService.LoginAsync(loginDto);
            return Ok(new TokenResponseDto { Token = token });
        }


        [HttpPost("register/admin")]
        [Authorize(Roles = "SuperAdministrator")]
        public async Task<IActionResult> RegisterAdministratorAsync([FromBody] AddUserDto addUserDto)
        {
            var newUser = await _authService.RegisterAdministratorAsync(addUserDto);

            return CreatedAtRoute("GetUserById", new { id = newUser.Id }, newUser);
        }

        [HttpPost("register/teacher")]
        [Authorize(Roles = "SuperAdministrator")]
        public async Task<IActionResult> RegisterTeacherAsync([FromBody] AddUserDto addUserDto)
        {
            var newUser = await _authService.RegisterTeacherAsync(addUserDto);

            return CreatedAtRoute("GetUserById", new { id = newUser.Id }, newUser);
        }
    }
}



public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
}