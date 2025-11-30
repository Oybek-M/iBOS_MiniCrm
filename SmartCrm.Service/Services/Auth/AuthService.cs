using AutoMapper;
using FluentValidation;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Entities.Users;
using SmartCrm.Domain.Enums;
using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.Common.Security;
using SmartCrm.Service.DTOs.Auth;
using SmartCrm.Service.DTOs.Users;
using SmartCrm.Service.Interfaces.Auth;
using System.Net;

namespace SmartCrm.Service.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;
        private readonly IValidator<AddUserDto> _addUserValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuthManager authManager,
            IValidator<AddUserDto> addUserValidator,
            IValidator<LoginDto> loginValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authManager = authManager;
            _addUserValidator = addUserValidator;
            _loginValidator = loginValidator;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));

            var user = await _unitOfWork.User.GetByUserNameAsync(loginDto.Username);
            if (user is null)
                throw new StatusCodeException(HttpStatusCode.Unauthorized, "Username yoki parol xato.");

            if (!user.IsActive)
                throw new StatusCodeException(HttpStatusCode.Forbidden, "Foydalanuvchi akkaunti faol emas.");

            bool isPasswordCorrect = PasswordHasher.IsEqual(user.PasswordHash, loginDto.Password);
            if (!isPasswordCorrect)
                throw new StatusCodeException(HttpStatusCode.Unauthorized, "Username yoki parol xato.");

            return _authManager.GenerateToken(user);
        }

        public async Task<UserDto> RegisterAdministratorAsync(AddUserDto addUserDto)
        {
            var validationResult = await _addUserValidator.ValidateAsync(addUserDto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));

            var existingUser = await _unitOfWork.User.GetByUserNameAsync(addUserDto.Username);
            if (existingUser is not null)
                throw new StatusCodeException(HttpStatusCode.Conflict, $"'{addUserDto.Username}' nomli foydalanuvchi allaqachon mavjud.");


            var newUser = new User
            {
                FirstName = addUserDto.FirstName,
                LastName = addUserDto.LastName,
                Username = addUserDto.Username,
                PasswordHash = PasswordHasher.GetHash(addUserDto.Password),
                Role = Role.Administrator,
                IsActive = true
            };

            await _unitOfWork.User.Add(newUser);

            return _mapper.Map<UserDto>(newUser);
        }
    }
}