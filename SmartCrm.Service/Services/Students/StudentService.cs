using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Enums;
using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.DTOs.Dashboard;
using SmartCrm.Service.DTOs.Students;
using SmartCrm.Service.Interfaces.Students;
using System.Net;

namespace SmartCrm.Service.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateStudentDto> _createValidator;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateStudentDto> createValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors));

            var group = await _unitOfWork.Group.GetById(dto.GroupId);
            if (group is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Bunday IDga ega guruh topilmadi.");

            //var existingStudent = await _unitOfWork.Student.FirstOrDefaultAsync(s => s.PhoneNumber == dto.PhoneNumber);
            //if (existingStudent is not null)
            //    throw new StatusCodeException(HttpStatusCode.Conflict, "Bu telefon raqami allaqachon ro'yxatdan o'tgan.");

            var newStudent = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                MonthlyPaymentAmount = dto.MonthlyPaymentAmount,
                DiscountPercentage = dto.DiscountPercentage,
                GroupId = dto.GroupId,
                PaymentStatus = MonthlyPaymentStatus.Unpaid
            };

            await _unitOfWork.Student.Add(newStudent);

            return await GetByIdAsync(newStudent.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await _unitOfWork.Student.GetById(id);

            if (student is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Bunday ID'ga ega o'quvchi topilmadi.");

            await _unitOfWork.Student.Remove(student);

            return true;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var students = await _unitOfWork.Student.GetAll()
                .Include(s => s.Group).ThenInclude(g => g.Teacher)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task<StudentDto> GetByIdAsync(Guid id)
        {
            var student = await _unitOfWork.Student.GetAll()
                .Include(s => s.Group).ThenInclude(g => g.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "O'quvchi topilmadi.");

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> UpdateAsync(Guid id, UpdateStudentDto dto)
        {
            var student = await _unitOfWork.Student.GetById(id);
            if (student is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "O'quvchi topilmadi.");

            var existingStudent = await _unitOfWork.Student.FirstOrDefaultAsync(s => s.PhoneNumber == dto.PhoneNumber && s.Id != id);
            if (existingStudent is not null)
                throw new StatusCodeException(HttpStatusCode.Conflict, "Bu telefon raqami boshqa o'quvchiga tegishli.");

            var group = await _unitOfWork.Group.GetById(dto.GroupId);
            if (group is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Bunday IDga ega guruh topilmadi.");

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.PhoneNumber = dto.PhoneNumber;
            student.MonthlyPaymentAmount = dto.MonthlyPaymentAmount;
            student.DiscountPercentage = dto.DiscountPercentage;
            student.GroupId = dto.GroupId;

            await _unitOfWork.Student.Update(student);

            return await GetByIdAsync(id);
        }

        public async Task<PaymentStatusFilterResultDto> GetStudentsByPaymentStatusAsync(MonthlyPaymentStatus status)
        {
            var query = _unitOfWork.Student.GetAll()
                .Where(s => s.PaymentStatus == status)
                .Include(s => s.Group).ThenInclude(g => g.Teacher);

            var students = await query.ToListAsync();
            var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);

            var result = new PaymentStatusFilterResultDto
            {
                Count = students.Count,
                TotalAmount = students.Sum(s => s.MonthlyPaymentAmount),
                Students = studentDtos
            };

            return result;
        }

        public async Task<IEnumerable<StudentDto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var lowerCaseSearchTerm = searchTerm.ToLower().Trim();

            var students = await _unitOfWork.Student.GetAll()
                .Where(s =>
                    s.FirstName.ToLower().Contains(lowerCaseSearchTerm) ||
                    s.LastName.ToLower().Contains(lowerCaseSearchTerm))
                .Include(s => s.Group).ThenInclude(g => g.Teacher) 
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }
    }
}