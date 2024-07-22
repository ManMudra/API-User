namespace Manmudra.DTO.Account
{
    public class AuthenticationDto: LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
