using SmartCrm.Domain.Enums;
using SmartCrm.Service.DTOs.Groups;

namespace SmartCrm.Service.DTOs.Students;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public decimal MonthlyPaymentAmount { get; set; }
    public int DiscountPercentage { get; set; }
    public MonthlyPaymentStatus PaymentStatus { get; set; }

    public decimal Balance { get; set; }
    public GroupDto Group { get; set; }
}
