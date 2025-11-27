using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces.Students;
using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Enums;

namespace SmartCrm.Data.Repositories.Students;

public class StudentRepository : Repository<Student>, IStudent
{
    private readonly AppDbContext _appDb;
    public StudentRepository(AppDbContext appDb) : base(appDb)
    {
        _appDb = appDb;
    }

    public async Task<IEnumerable<Student>> GetStudentsByPaymentStatusAsync(MonthlyPaymentStatus status)
    {
        var students = await _appDb.Students
            .Where(s => s.PaymentStatus == status)
            .Include(s => s.Group)
            .ToListAsync();

        return students;
    }
}
