namespace Manmudra.DTO.Address
{
    public class AddressDto
    {
        public int Id { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? SubDistrictId { get; set; }
        public int? BlockId { get; set; }
        public int? PanchayatId { get; set; }
        public string? CityOrVillage { get; set; }
        public int? PinCode { get; set; }
        public string? HouseNo { get; set; }
        public string? LandMark { get; set; }
        public string? PostOffice { get; set; }
        public string? Email { get; set; }
        public string? CreatedBy { get; set; }
        public bool IsUrban { get; set; }
    }
}
