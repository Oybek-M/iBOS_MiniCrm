using SmartCrm.Service.DTOs.Students;

namespace SmartCrm.Service.DTOs.Dashboard;

public class PaymentStatusFilterResultDto
{
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
    public IEnumerable<StudentDto> Students { get; set; }
}
