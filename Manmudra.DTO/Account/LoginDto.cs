namespace Manmudra.DTO.Account
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Last4Digit { get; set; }
        public string? MobileNo { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool? IsProduction { get; set; }
        public bool? CheckAadhaar { get; set; }
    }
}
