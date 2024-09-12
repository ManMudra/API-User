namespace Manmudra.DTO.Account
{
    public class ForgetPinDto
    {
        public string? MobileNo { get; set; }
        public string? Country { get; set; }
        public string? Language { get; set; }
        public string? Otp { get; set; }
        public int? UnionId { get; set; }
        public bool? IgnoreUnion { get; set; }
        public bool? UpdateVerification { get; set; }
        public bool Cancelled { get; set; } = false;
    }
}
