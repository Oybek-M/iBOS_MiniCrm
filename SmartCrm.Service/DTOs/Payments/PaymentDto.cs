using SmartCrm.Domain.Enums;
using SmartCrm.Service.DTOs.Students;

namespace SmartCrm.Service.DTOs.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentType Type { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Notes { get; set; }

    public StudentDto Student { get; set; }
}

