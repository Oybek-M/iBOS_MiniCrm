using SmartCrm.Service.DTOs.Groups;

namespace SmartCrm.Service.DTOs.Teachers;

public class TeacherDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }

    public ICollection<GroupDto> Groups { get; set; }
}
