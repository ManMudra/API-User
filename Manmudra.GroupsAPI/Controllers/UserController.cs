using Manmudra.DTO.Account;
using Manmudra.DTO.Address;
using Manmudra.DTO.Response;
using Manmudra.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manmudra.GroupsAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [AllowAnonymous]
        [HttpGet("GetUserById")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(string id)
        {
            var response = await this.userService.GetUserById(id);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetApplicationRoleByUser")]
        public async Task<ActionResult<ApiResponse<List<ApplicationUserRolesDto>>>> GetApplicationRoleByUser(string id, int unionId)
        {
            var response = await this.userService.GetApplicationRoleByUser(id, unionId);
            return Ok(response);
        }

        [HttpPost("GetApplicationRoleByUserIds")]
        public async Task<ActionResult<ApiResponse<List<ApplicationUserRolesDto>>>> GetApplicationRoleByUserIds(IdsDto ids)
        {
            var response = await this.userService.GetApplicationRoleByUserIds(ids);
            return Ok(response);
        }

        [HttpGet("GetApplicationRoleByUnion")]
        public async Task<ActionResult<ApiResponse<List<ApplicationUserRolesDto>>>> GetApplicationRoleByUnion(int unionId)
        {
            var response = await this.userService.GetApplicationRoleByUnion(unionId);
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

        [HttpGet("GetUsersByUnion")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsersByUnion(int unionId)
        {
            var response = await this.userService.GetUsersByUnion(unionId);
            return Ok(response);
        }

        [HttpPost("GetAddressByIds")]
        public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetAddressByIds(IdsDto ids)
        {
            var response = await this.userService.GetAddressByIds(ids);
            return Ok(response);
        }

        [HttpPost("GetUserByIds")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUserByIds(IdsDto ids)
        {
            var response = await this.userService.GetUserByIds(ids);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetUserByPhoneNumber")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByPhoneNumber(string phoneNumber)
        {
            var response = await this.userService.GetUserByPhoneNumber(phoneNumber);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetUsersByPhoneNumber")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsersByPhoneNumber(string phoneNumber)
        {
            var response = await this.userService.GetUsersByPhoneNumber(phoneNumber);
            return Ok(response);
        }

        [HttpGet("ValidateAadharNumber")]
        public async Task<ActionResult<ApiResponse<UserDto>>> ValidateAadharNumber(string id, int unionId)
        {
            var response = await this.userService.ValidateAadharNumber(id, unionId);
            return Ok(response);
        }

        [HttpPost("MigrateDataToServer")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> MigrateDataToServer(MigrateDto migrate)
        {
            var response = await this.userService.MigrateDataToServer(migrate);
            return Ok(response);
        }

        [HttpPost("SearchVillage")]
        public async Task<ActionResult<ApiResponse<List<string>>>> SearchVillage(AddressSearchDto model)
        {
            var response = await this.userService.SearchVillage(model);
            return Ok(response);
        }

        [HttpGet("SearchCaste")]
        public async Task<ActionResult<ApiResponse<List<string>>>> SearchCaste(string searchText)
        {
            var response = await this.userService.SearchCaste(searchText);
            return Ok(response);
        }

        [HttpGet("TotalUsersCount")]
        public async Task<ActionResult<ApiResponse<int>>> TotalUsersCount(long unionId, string? panchayatIds)
        {
            var response = await this.userService.TotalUsersCount(unionId, panchayatIds);
            return Ok(response);
        }

        [HttpGet("TotalTransactionCount")]
        public async Task<ActionResult<ApiResponse<List<string>>>> TotalTransactionCount(long unionId, string? panchayatIds)
        {
            var response = await this.userService.TotalTransactionCount(unionId, panchayatIds);
            return Ok(response);
        }

        [HttpGet("GetUnionUserStates")]
        public async Task<ActionResult<ApiResponse<List<int>>>> GetUnionUserStates(long unionId)
        {
            var response = await this.userService.GetUnionUserStates(unionId);
            return Ok(response);
        }

        [HttpGet("GetMemberCountInUnion")]
        public async Task<ActionResult<ApiResponse<int>>> GetMemberCountInUnion(long unionId)
        {
            var response = await this.userService.GetMemberCountInUnion(unionId);
            return Ok(response);
        }

        [HttpGet("MigrateDataBackFromServer")]
        public async Task<ActionResult<ApiResponse<MigrateDataBackFromServerDto>>> MigrateDataBackFromServer(int unionId)
        {
            var response = await this.userService.MigrateDataBackFromServer(unionId);
            return Ok(response);
        }
    }
}
