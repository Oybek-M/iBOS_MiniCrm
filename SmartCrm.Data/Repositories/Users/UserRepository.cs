using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces.Users;
using SmartCrm.Domain.Entities.Users;

namespace SmartCrm.Data.Repositories.Users;

public class UserRepository : Repository<User>, IUser
{
    private readonly AppDbContext _appDb;

    public UserRepository(AppDbContext appDb) : base(appDb)
    {
        _appDb = appDb;
    }
    public async Task<User?> GetByUserNameAsync(string userName)
    {
        var user = await _appDb.Users
            .FirstOrDefaultAsync(u => u.Username == userName);
        return user;
    }
}
