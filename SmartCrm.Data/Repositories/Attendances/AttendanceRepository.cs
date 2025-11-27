using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces.Attendances;
using SmartCrm.Domain.Entities.Attendances;

namespace SmartCrm.Data.Repositories.Attendances
{
    public class AttendanceRepository(AppDbContext appDbContext)
        : Repository<Attendance>(appDbContext), IAttendances
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        public async Task<IEnumerable<Attendance>> GetAttendancesByGroup(Guid groupId)
        {
            var attendances = await _appDbContext.Attendances
                .Where(s => s.Student.GroupId == groupId)
                .Include(s => s.Student)
                .ToListAsync();

            return attendances;
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByStudent(Guid studentId)
        {
            var attendances = await _appDbContext.Attendances
                .Where(a => a.StudentId == studentId)
                .Include(a => a.Student)
                .ToListAsync();

            return attendances;
        }
    }
}
