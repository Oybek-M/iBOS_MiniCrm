using SmartCrm.Service.Common.Validators.Auth;
using SmartCrm.Service.Common.Validators.Groups;
using SmartCrm.Service.Common.Validators.Payments;
using SmartCrm.Service.Common.Validators.Students;
using SmartCrm.Service.Common.Validators.Teachers;
using SmartCrm.Service.Common.Validators.Users;
using SmartCrm.Service.DTOs.Auth;
using SmartCrm.Service.DTOs.Groups;
using SmartCrm.Service.DTOs.Payments;
using SmartCrm.Service.DTOs.Students;
using SmartCrm.Service.DTOs.Teachers;
using SmartCrm.Service.DTOs.Users;
using FluentValidation;
using SmartCrm.Service.DTOs.Attendances;
using SmartCrm.Service.Common.Validators.Attendances;

namespace SmartCrm.Configurations;

public static class ValidatorsConfiguration
{
    public static void ConfigurationValidators(this WebApplicationBuilder builder)
    {
        // Group Validators
        builder.Services.AddScoped<IValidator<CreateGroupDto>, CreateGroupValidator>();
        builder.Services.AddScoped<IValidator<UpdateGroupDto>, UpdateGroupValidator>();

        builder.Services.AddScoped<IValidator<LoginDto>, LoginValidator>();
        // Payment Validators
        builder.Services.AddScoped<IValidator<CreatePaymentDto>, CreatePaymentValidator>();

        // Student Validators
        builder.Services.AddScoped<IValidator<CreateStudentDto>, CreateStudentValidator>();

        // Attendance Validators
        builder.Services.AddScoped<IValidator<CreateAttendanceDto>, CreateAttendanceValidator>();

        // Teacher Validators
        builder.Services.AddScoped<IValidator<CreateTeacherDto>, CreateTeacherValidator>();

        // User Validators
        builder.Services.AddScoped<IValidator<AddUserDto>, AddUserValidator>();
        builder.Services.AddScoped<IValidator<UpdateUserDto>, UpdateUserValidator>();
        builder.Services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordValidator>();
    }
}
