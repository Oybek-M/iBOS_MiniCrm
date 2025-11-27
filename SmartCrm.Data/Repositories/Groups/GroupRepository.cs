using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces.Groups;
using SmartCrm.Domain.Entities.Groups;

namespace SmartCrm.Data.Repositories.Groups;

public class GroupRepository : Repository<Group>, IGroup
{
    public GroupRepository(AppDbContext appDb) : base(appDb)
    { }
}
