using SmartCrm.Domain.Enums;

namespace SmartCrm.Service.DTOs.Attendances
{
    public class CreateAttendanceDto
    {
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}
