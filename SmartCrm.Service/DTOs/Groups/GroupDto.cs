using SmartCrm.Service.DTOs.Students;
using SmartCrm.Service.DTOs.Teachers;

namespace SmartCrm.Service.DTOs.Groups;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public TeacherForResultDto Teacher { get; set; }

    public ICollection<StudentDto> Students { get; set; }
}
