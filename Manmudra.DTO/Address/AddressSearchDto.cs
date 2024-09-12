namespace Manmudra.DTO.Address
{
    public class AddressSearchDto
    {
        public string? Text { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? SubDistrictId { get; set; }
        public int? BlockId { get; set; }
        public int? PanchayatId { get; set; }
    }
}
