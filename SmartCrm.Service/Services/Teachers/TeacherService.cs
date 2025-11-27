using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Entities.Teachers;
using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.DTOs.Teachers;
using SmartCrm.Service.Interfaces.Teachers;
using System.Net;

namespace SmartCrm.Service.Services.Teachers
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateTeacherDto> _createValidator;

        public TeacherService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateTeacherDto> createValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        public async Task<TeacherDto> CreateAsync(CreateTeacherDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors));

            var existingTeacher = await _unitOfWork.Teacher.FirstOrDefaultAsync(t => t.PhoneNumber == dto.PhoneNumber);
            if (existingTeacher is not null)
                throw new StatusCodeException(HttpStatusCode.Conflict, "Bu telefon raqami allaqachon ro'yxatdan o'tgan.");

            var newTeacher = new Teacher
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };
            await _unitOfWork.Teacher.Add(newTeacher);

            return _mapper.Map<TeacherDto>(newTeacher);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var teacher = await _unitOfWork.Teacher.GetById(id);
            if (teacher is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "O'qituvchi topilmadi.");

            bool hasGroups = await _unitOfWork.Group.GetAll().AnyAsync(g => g.TeacherId == id);
            if (hasGroups)
                throw new StatusCodeException(HttpStatusCode.BadRequest, "O'qituvchini o'chirib bo'lmaydi, chunki unga biriktirilgan guruhlar mavjud.");

            await _unitOfWork.Teacher.Remove(teacher);
            return true;
        }

        public async Task<IEnumerable<TeacherDto>> GetAllAsync()
        {
            var teachers = await _unitOfWork.Teacher.GetAll()
                .Include(t => t.Groups) 
                .ToListAsync();

            return _mapper.Map<IEnumerable<TeacherDto>>(teachers);
        }

        public async Task<TeacherDto> GetByIdAsync(Guid id)
        {
            var teacher = await _unitOfWork.Teacher.GetAll()
                .Include(t => t.Groups)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "O'qituvchi topilmadi.");

            return _mapper.Map<TeacherDto>(teacher);
        }

        public async Task<TeacherDto> UpdateAsync(Guid id, UpdateTeacherDto dto)
        {
            var teacher = await _unitOfWork.Teacher.GetById(id);
            if (teacher is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "O'qituvchi topilmadi.");

            var existingTeacher = await _unitOfWork.Teacher.FirstOrDefaultAsync(t => t.PhoneNumber == dto.PhoneNumber && t.Id != id);
            if (existingTeacher is not null)
                throw new StatusCodeException(HttpStatusCode.Conflict, "Bu telefon raqami boshqa o'qituvchiga tegishli.");

            teacher.FirstName = dto.FirstName;
            teacher.LastName = dto.LastName;
            teacher.PhoneNumber = dto.PhoneNumber;

            await _unitOfWork.Teacher.Update(teacher);

            return _mapper.Map<TeacherDto>(teacher);
        }
    }
}