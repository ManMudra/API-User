using System.Net;

namespace Manmudra.DTO.Account
{
    public class LoginResponseDto
    {
        public UserDto? User { get; set; }
        public bool VerifiedUser { get; set; } = true;
        public bool BlockedUser { get; set; } = false;
        public string? JwtToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime Valid { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool AddToOtherUnion { get; set; }
        public bool VerifyUnionTimeout { get; set; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
    }
}
