using SmartCrm.Domain.Enums;

namespace SmartCrm.Service.DTOs.Payments;

public class CreatePaymentDto
{
    public Guid StudentId { get; set; }
    public decimal AmountPaid { get; set; }
    public PaymentType Type { get; set; }
    public string? Notes { get; set; }
}
