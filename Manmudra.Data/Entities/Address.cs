using Manmudra.Data.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manmudra.Data.Entities
{
    public class Address : Stamp
    {
        public int Id { get; set; }
        public string? HouseNo { get; set; }
        public bool IsUrban { get; set; }
        public string? LandMark { get; set; }
        public string? CityOrVillage { get; set; }
        public string? PostOffice { get; set; }
        [Column(TypeName = "varchar(320)")]
        public string? Email { get; set; }
        public int? PanchayatId { get; set; }
        public int? BlockId { get; set; }
        public int? SubDistrictId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? PinCode { get; set; }
        public int Multiple { get; set; }
        public int? OldAddressId { get; set; }
    }
}
