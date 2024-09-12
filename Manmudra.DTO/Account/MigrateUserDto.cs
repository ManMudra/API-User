namespace Manmudra.DTO.Account
{
    public class MigrateUserDto: UserDto
    {
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
    }
}
