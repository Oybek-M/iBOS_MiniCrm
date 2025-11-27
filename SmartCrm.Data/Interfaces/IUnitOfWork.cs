using SmartCrm.Data.Interfaces.Attendances;
using SmartCrm.Data.Interfaces.Groups;
using SmartCrm.Data.Interfaces.Payments;
using SmartCrm.Data.Interfaces.Students;
using SmartCrm.Data.Interfaces.Teachers;
using SmartCrm.Data.Interfaces.Users;

namespace SmartCrm.Data.Interfaces;

public interface IUnitOfWork
{
    IGroup Group { get; set; }
    IPayment Payment { get; set; }
    IStudent Student { get; set; }
    ITeacher Teacher { get; set; }
    IUser User { get; set; }
    IAttendances Attendances { get; set; }
}
