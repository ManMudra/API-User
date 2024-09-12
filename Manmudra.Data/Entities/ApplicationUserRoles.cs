using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manmudra.Data.Entities
{
    public class ApplicationUserRoles
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "varchar(36)")]
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public int UnionId { get; set; }
    }
}
