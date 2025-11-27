using SmartCrm.Data.Interfaces;
using SmartCrm.Data.Interfaces.Attendances;
using SmartCrm.Data.Repositories;
using SmartCrm.Service.BackgroundJobs;
using SmartCrm.Service.Interfaces.Attendances;
using SmartCrm.Service.Interfaces.Auth;
using SmartCrm.Service.Interfaces.Groups;
using SmartCrm.Service.Interfaces.Payments;
using SmartCrm.Service.Interfaces.Students;
using SmartCrm.Service.Interfaces.Teachers;
using SmartCrm.Service.Interfaces.Users;
using SmartCrm.Service.Services.Attendances;
using SmartCrm.Service.Services.Auth;
using SmartCrm.Service.Services.Groups;
using SmartCrm.Service.Services.Payments;
using SmartCrm.Service.Services.Students;
using SmartCrm.Service.Services.Teachers;
using SmartCrm.Service.Services.Users;

namespace SmartCrm.Configurations
{
    public static class ServiceLayerConfiguration
    {
        public static void ConfigureServiceLayer(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthManager, AuthManager>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddHostedService<MonthlyPaymentResetJob>();
        }
    }
}