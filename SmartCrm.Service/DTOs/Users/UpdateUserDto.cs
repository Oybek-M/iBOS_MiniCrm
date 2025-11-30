namespace SmartCrm.Service.DTOs.Users;

public class UpdateUserDto
{

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}
