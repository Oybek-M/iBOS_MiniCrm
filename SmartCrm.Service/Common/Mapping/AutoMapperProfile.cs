using SmartCrm.Domain.Entities.Groups;
using SmartCrm.Domain.Entities.Payments;
using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Entities.Teachers;
using SmartCrm.Domain.Entities.Users;
using SmartCrm.Service.DTOs.Groups;
using SmartCrm.Service.DTOs.Payments;
using SmartCrm.Service.DTOs.Students;
using SmartCrm.Service.DTOs.Teachers;
using SmartCrm.Service.DTOs.Users;
using AutoMapper;
using SmartCrm.Domain.Entities.Attendances;
using SmartCrm.Service.DTOs.Attendances;

namespace SmartCrm.Service.Common.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<Group, GroupDto>();

            CreateMap<Student, StudentDto>();

            CreateMap<Teacher, TeacherDto>();
            CreateMap<Teacher, TeacherForResultDto>();

            CreateMap<Payment, PaymentDto>();
            CreateMap<Attendance, CreateAttendanceDto>().ReverseMap();
        }
    }
}
