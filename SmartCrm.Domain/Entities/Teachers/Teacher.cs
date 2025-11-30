using SmartCrm.Domain.Entities.Groups;
using SmartCrm.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCrm.Domain.Entities.Teachers
{
    [Table("teachers")]
    public class Teacher : BaseEntity
    {
        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        [Column("password_hash")]
        public string Password { get; set; }

        public Role Role { get; set; } = Role.Teacher;

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}