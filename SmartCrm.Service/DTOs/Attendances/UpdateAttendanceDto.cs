using SmartCrm.Domain.Enums;

namespace SmartCrm.Service.DTOs.Attendances
{
    public class UpdateAttendanceDto
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}
