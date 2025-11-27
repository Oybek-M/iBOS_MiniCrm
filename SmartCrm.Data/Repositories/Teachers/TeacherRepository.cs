using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces.Teachers;
using SmartCrm.Domain.Entities.Teachers;

namespace SmartCrm.Data.Repositories.Teachers;

public class TeacherRepository : Repository<Teacher>, ITeacher
{
    public TeacherRepository(AppDbContext appDb) : base(appDb)
    {  }
}
