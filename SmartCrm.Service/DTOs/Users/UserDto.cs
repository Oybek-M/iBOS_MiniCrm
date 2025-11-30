using SmartCrm.Domain.Enums;

namespace SmartCrm.Service.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public Role Role { get; set; }
    public bool IsActive { get; set; }
}
