using SmartCrm.Domain.Entities.Users;

namespace SmartCrm.Service.Interfaces.Auth
{
    public interface IAuthManager
    {
        string GenerateToken(User user);
    }
}
