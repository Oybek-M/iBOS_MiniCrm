using SmartCrm.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCrm.Domain.Entities.Users;

[Table("users")]
public class User : BaseEntity
{
    [Column("first_name")]
    public string FirstName { get; set; }

    [Column("last_name")]
    public string LastName { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; }

    [Column("role")]
    public Role Role { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}
