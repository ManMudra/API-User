namespace Manmudra.DTO.Account
{
    public class MigrateDataBackFromServerDto
    {
        public List<MigrateUserDto> Users { get; set; }
        public List<ApplicationUserRolesDto> ApplicationUserRoles { get; set; }
    }
}
