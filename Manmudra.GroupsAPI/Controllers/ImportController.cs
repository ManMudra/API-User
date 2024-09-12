using Manmudra.Data.Context;
using Manmudra.Data.Entities;
using Manmudra.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Manmudra.GroupsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController(ManmudraContext dbContext) : ControllerBase
    {
        private readonly ManmudraContext dbContext = dbContext;

        [AllowAnonymous]
        [HttpPost("AddInitialData")]
        public async Task<ActionResult<ApiResponse<bool>>> AddInitialData()
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            var usersPath = "E:\\ManMudraDatabase\\Users.json";
            var userRolesPath = "E:\\ManMudraDatabase\\ApplicationUserRoles.json";
            var addressPath = "E:\\ManMudraDatabase\\Addresses.json";
            var rolesPath = "E:\\ManMudraDatabase\\Roles.json";

            var users = JsonConvert.DeserializeObject<List<ApplicationUser>>(System.IO.File.ReadAllText(usersPath));
            var userRoles = JsonConvert.DeserializeObject<List<ApplicationUserRoles>>(System.IO.File.ReadAllText(userRolesPath));
            var address = JsonConvert.DeserializeObject<List<Address>>(System.IO.File.ReadAllText(addressPath));
            var roles = JsonConvert.DeserializeObject<List<IdentityRole>>(System.IO.File.ReadAllText(rolesPath));
            
            await dbContext.Users.AddRangeAsync(users);
            await dbContext.ApplicationUserRoles.AddRangeAsync(userRoles);
            await dbContext.Addresses.AddRangeAsync(address);
            await dbContext.Roles.AddRangeAsync(roles);

            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new ApiResponse<bool>
            {
                IsSuccess = true,
            });
        }
    }
}
