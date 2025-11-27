using SmartCrm.Domain.Entities.Users;

namespace SmartCrm.Data.Interfaces.Users;

public interface IUser : IRepository<User>
{
    public Task<User?> GetByUserNameAsync(string userName);
}
