using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Entities.Teachers;
using SmartCrm.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCrm.Domain.Entities.Attendances
{
    [Table("attendances")]
    public class Attendance : BaseEntity
    {
        [Required]
        [Column("student_id")]
        public Guid StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        [Required]
        [Column("teacher_id")]
        public Guid TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("status")]
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        [Column("note")]
        public string? Note { get; set; }
    }
}
