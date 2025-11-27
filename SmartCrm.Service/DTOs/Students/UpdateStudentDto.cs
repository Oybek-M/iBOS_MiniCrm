namespace SmartCrm.Service.DTOs.Students;

public class UpdateStudentDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public decimal MonthlyPaymentAmount { get; set; }
    public int DiscountPercentage { get; set; }
    public Guid GroupId { get; set; }
}
