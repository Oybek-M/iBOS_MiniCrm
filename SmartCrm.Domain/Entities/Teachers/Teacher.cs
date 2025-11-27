using SmartCrm.Domain.Entities.Groups;
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

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}