using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCrm.Domain.Entities;

public abstract class BaseEntity
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("created_at")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Yaxshi serviceda +5 qilamiz

    [Column("updated_at")]
    public DateTime? UpdatedDate { get; set; }
}