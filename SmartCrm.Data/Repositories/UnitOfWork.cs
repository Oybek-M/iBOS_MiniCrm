using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces;
using SmartCrm.Data.Interfaces.Attendances;
using SmartCrm.Data.Interfaces.Groups;
using SmartCrm.Data.Interfaces.Payments;
using SmartCrm.Data.Interfaces.Students;
using SmartCrm.Data.Interfaces.Teachers;
using SmartCrm.Data.Interfaces.Users;
using SmartCrm.Data.Repositories.Attendances;
using SmartCrm.Data.Repositories.Groups;
using SmartCrm.Data.Repositories.Payments;
using SmartCrm.Data.Repositories.Students;
using SmartCrm.Data.Repositories.Teachers;
using SmartCrm.Data.Repositories.Users;

namespace SmartCrm.Data.Repositories;

public class UnitOfWork(AppDbContext appDb) : IUnitOfWork, IDisposable
{
    public IGroup Group { get; set; } = new GroupRepository(appDb);
    public IPayment Payment { get; set; } = new PaymentRepository(appDb);
    public IStudent Student { get; set; } = new StudentRepository(appDb);
    public ITeacher Teacher { get; set; } = new TeacherRepository(appDb);
    public IUser User { get; set; } = new UserRepository(appDb);
    public IAttendances Attendances { get; set; } = new AttendanceRepository(appDb);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
