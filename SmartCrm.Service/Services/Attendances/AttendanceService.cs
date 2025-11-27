using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Entities.Attendances;
using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.DTOs.Attendances;
using SmartCrm.Service.Interfaces.Attendances;
using System.Net;

namespace SmartCrm.Service.Services.Attendances
{
    public class AttendanceService(IUnitOfWork unitOfWork,
                                   IMapper mapper,
                                   IValidator<CreateAttendanceDto> createValidator)
        : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateAttendanceDto> _createValidator = createValidator;


        public async Task<Attendance> CreateAsync(CreateAttendanceDto dto)
        {
            var validationRes = await _createValidator.ValidateAsync(dto);
            if(!validationRes.IsValid)
                throw new ValidationException(string.Join("\n", validationRes.Errors));

            var student = await _unitOfWork.Student.GetById(dto.StudentId);
            if (student is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Bunday ID ga ega O'quvchi topilmadi");

            var teacher = await _unitOfWork.Teacher.GetById(dto.TeacherId);
            if (teacher is null) throw new StatusCodeException(HttpStatusCode.NotFound, "Buunday ID ga ega O'qituvchi topilmadi");

            var newAttendance = new Attendance
            {
                StudentId = dto.StudentId,
                Student = student,
                TeacherId = dto.TeacherId,
                Teacher = teacher,
                Status = dto.Status,
                Date = dto.Date,
                Note = dto.Note

            };

            await _unitOfWork.Attendances.Add(newAttendance);

            return await GetByIdAsync(newAttendance.Id);
        }

        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            var attendances = await _unitOfWork.Attendances.GetAll()
                                                     .Include(a => a.Student)
                                                     .ThenInclude(a => a.Group)
                                                     .ToListAsync();

            return attendances;
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByGroupIdAsync(Guid groupId)
        {
            var group = _unitOfWork.Group.GetById(groupId);
            if (group is null)
                throw new StatusCodeException(HttpStatusCode.NotFound,
                                              "Ushbu ID ga tegishli guruh topilamdi");

            var attendances = await _unitOfWork.Attendances.GetAttendancesByGroup(groupId);
            return attendances;
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByStudentIdAsync(Guid studentId)
        {
            var student = _unitOfWork.Student.GetById(studentId);
            if (student is null)
                throw new StatusCodeException(HttpStatusCode.NotFound,
                                              "Ushbu ID ga tegishli o'quvchi topilmadi");

            var attendances = await _unitOfWork.Attendances.GetAttendancesByStudent(studentId);
            return attendances;
        }

        public async Task<Attendance> GetByIdAsync(Guid id)
        {
            var attendance = await _unitOfWork.Attendances.GetAll()
                                                           .Include(a => a.Student)
                                                           .ThenInclude(a => a.Group)
                                                           .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance is null)
                throw new StatusCodeException(HttpStatusCode.NotFound,
                                              "Ushbu ID ga tegishli davomat topilmadi.");

            return attendance;
        }

        public async Task<Attendance> UpdateAsync(Guid id, UpdateAttendanceDto dto)
        {
            var attendace = await _unitOfWork.Attendances.GetById(id);
            if (attendace is null)
                throw new StatusCodeException(HttpStatusCode.NotFound,
                                              "Ushbu ID ga tegishli davomat topilmadi");

            attendace.Status = dto.Status;
            attendace.Date = dto.Date;
            attendace.Note = dto.Note;

            await _unitOfWork.Attendances.Update(attendace);
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var attendance = await _unitOfWork.Attendances.GetById(id);
            if (attendance is null)
                throw new StatusCodeException(HttpStatusCode.NotFound,
                                              "Ushbu ID ga ega davomat topilmadi");

            await _unitOfWork.Attendances.Remove(attendance);
            return true;
        }
    }
}
