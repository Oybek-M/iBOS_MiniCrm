using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCrm.Domain.Entities.Payments;

[Table("payments")]
public class Payment : BaseEntity
{
    [Column("amount", TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Column("payment_type")]
    public PaymentType Type { get; set; }

    [Column("payment_date")]
    public DateTime PaymentDate { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("student_id")]
    public Guid StudentId { get; set; }

    [ForeignKey("StudentId")]
    public Student Student { get; set; }
}