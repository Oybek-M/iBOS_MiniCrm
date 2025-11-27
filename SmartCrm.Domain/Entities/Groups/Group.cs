using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Entities.Teachers;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCrm.Domain.Entities.Groups;

[Table("groups")]
public class Group : BaseEntity
{
    [Column("name")]
    public string Name { get; set; }

    [Column("teacher_id")]
    public Guid TeacherId { get; set; }

    [ForeignKey("TeacherId")]
    public Teacher Teacher { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
