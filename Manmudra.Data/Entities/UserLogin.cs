using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manmudra.Data.Entities
{
    public class UserLogin
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(36)")]
        public string UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int Count { get; set; }
    }
}
