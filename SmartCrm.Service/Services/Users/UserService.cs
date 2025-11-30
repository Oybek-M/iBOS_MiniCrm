using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Enums;
using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.Common.Security;
using SmartCrm.Service.DTOs.Users;
using SmartCrm.Service.Interfaces.Users;
using System.Net;

namespace SmartCrm.Service.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateUserDto> _updateValidator;
        private readonly IValidator<ChangePasswordDto> _passwordValidator;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<UpdateUserDto> updateValidator,
            IValidator<ChangePasswordDto> passwordValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _updateValidator = updateValidator;
            _passwordValidator = passwordValidator;
        }

        public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto)
        {
            var validationResult = await _passwordValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors));

            var user = await _unitOfWork.User.GetById(id);
            if (user is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Foydalanuvchi topilmadi.");

            user.PasswordHash = PasswordHasher.GetHash(dto.NewPassword);
            await _unitOfWork.User.Update(user);
            return true;
        }

        public async Task<UserDto> ChangeStatusAsync(Guid id, bool isActive)
        {
            var user = await _unitOfWork.User.GetById(id);
            if (user is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Foydalanuvchi topilmadi.");

            if (user.Role == Role.SuperAdministrator)
                throw new StatusCodeException(HttpStatusCode.Forbidden, "Super Administrator statusini o'zgartirish mumkin emas.");

            user.IsActive = isActive;
            await _unitOfWork.User.Update(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _unitOfWork.User.GetById(id);
            if (user is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Foydalanuvchi topilmadi.");

            if (user.Role == Role.SuperAdministrator)
                throw new StatusCodeException(HttpStatusCode.Forbidden, "Super Administratorni o'chirib bo'lmaydi.");

            await _unitOfWork.User.Remove(user);
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllAdminsAsync()
        {
            var admins = await _unitOfWork.User.GetAll()
                .Where(u => u.Role == Role.Administrator)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(admins);
        }

        public async Task<IEnumerable<UserDto>> GetAllTeachersAsync()
        {
            var teachers = await _unitOfWork.User.GetAll()
                .Where(u => u.Role == Role.Teacher)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(teachers);
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.User.GetById(id);
            if (user is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Foydalanuvchi topilmadi.");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors));

            var user = await _unitOfWork.User.GetById(id);
            if (user is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Foydalanuvchi topilmadi.");

            if (user.Role == Role.SuperAdministrator)
                throw new StatusCodeException(HttpStatusCode.Forbidden, "Super Administrator ma'lumotlarini o'zgartirish mumkin emas.");

            var existingUser = await _unitOfWork.User.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Id != id);
            if (existingUser is not null)
                throw new StatusCodeException(HttpStatusCode.Conflict, "Bu username allaqachon band.");

            var changingPassword = !string.IsNullOrWhiteSpace(dto.OldPassword) || !string.IsNullOrWhiteSpace(dto.NewPassword);

            if (changingPassword)
            {
                // both must be provided to change password
                if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                    throw new StatusCodeException(HttpStatusCode.BadRequest, "Parolni o'zgartirish uchun eski va yangi parollarni to'liq kiriting.");

                var oldPasswordHash = PasswordHasher.GetHash(dto.OldPassword);
                if (oldPasswordHash != user.PasswordHash)
                    throw new StatusCodeException(HttpStatusCode.NotAcceptable, "Eski parolingiz mos kelmayapti.");

                // set new password
                user.PasswordHash = PasswordHasher.GetHash(dto.NewPassword);
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Username = dto.Username;
            await _unitOfWork.User.Update(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<int> GetTotalStudentCountAsync()
        {
            return await _unitOfWork.Student.GetAll().CountAsync();
        }
    }
}