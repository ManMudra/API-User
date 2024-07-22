using System.ComponentModel.DataAnnotations.Schema;

namespace Manmudra.Data.Base
{
    public class Stamp : IStamp
    {
        [Column(TypeName = "varchar(36)")]
        public string? CreatedBy { get; set; }
        [Column(TypeName = "varchar(36)")]
        public string? LastUpdatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public interface IStamp
    {
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
