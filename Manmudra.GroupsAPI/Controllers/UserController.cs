using Manmudra.DTO.Account;
using Manmudra.DTO.Response;
using Manmudra.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Manmudra.GroupsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [HttpGet("GetUserById")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(string id)
        {
            var response = await this.userService.GetUserById(id);
            return Ok(response);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(UserDto user)
        {
            var response = await this.userService.CreateUser(user);
            return Ok(response);
        }

        [HttpPost("UpdateUser")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(UserDto user)
        {
            var response = await this.userService.UpdateUser(user);
            return Ok(response);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<ApiResponse<UserDto>>> DeleteUser(string id)
        {
            var response = await this.userService.DeleteUser(id);
            return Ok(response);
        }

        [HttpPost("AddUserToUnionRole")]
        public async Task<ActionResult<ApiResponse<bool>>> AddUserToUnionRole(RolesDto roles)
        {
            var response = await this.userService.AddUserToUnionRole(roles);
            return Ok(response);
        }

        [HttpPost("GetUserListIds")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUserListIds(UserFilterDto userFilter)
        {
            var response = await this.userService.GetUserListIds(userFilter);
            return Ok(response);
        }
    }
}
