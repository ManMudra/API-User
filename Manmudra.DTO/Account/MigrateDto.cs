namespace Manmudra.DTO.Account
{
    public class MigrateDto
    {
        public List<MigrateUserDto>? Users { get; set; }
        public List<AppRolesDto>? Roles { get; set; }
        public List<ApplicationUserRolesDto>? ApplicationUserRoles { get; set; }
    }
}
