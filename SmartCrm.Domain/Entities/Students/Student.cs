using SmartCrm.Domain.Entities.Groups;
using SmartCrm.Domain.Entities.Payments;
using SmartCrm.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCrm.Domain.Entities.Students;

[Table("students")]
public class Student : BaseEntity
{
    [Column("first_name")]
    public string FirstName { get; set; }

    [Column("last_name")]
    public string LastName { get; set; }

    [Column("phone_number")]
    public string PhoneNumber { get; set; }

    [Column("monthly_payment_amount", TypeName = "decimal(18, 2)")]
    public decimal MonthlyPaymentAmount { get; set; }

    [Column("discount_percentage")]
    public int DiscountPercentage { get; set; }
    [Column("balance", TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; } = 0;

    [Column("monthly_payment_status")]
    public MonthlyPaymentStatus PaymentStatus { get; set; }

    [Column("group_id")]
    public Guid GroupId { get; set; }

    [ForeignKey("GroupId")]
    public Group Group { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
