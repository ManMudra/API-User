using Manmudra.Data.Entities;
using Manmudra.DTO.Account;
using Manmudra.DTO.Response;

namespace Manmudra.Services.Interface
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserById(string id);
        Task<ApiResponse<UserDto>> CreateUser(UserDto user);
        Task<ApiResponse<UserDto>> UpdateUser(UserDto user);
        Task<ApiResponse<bool>> DeleteUser(string id);
        Task<ApiResponse<List<UserDto>>> GetUserListIds(UserFilterDto userFilter);
        Task<ApiResponse<bool>> AddUserToUnionRole(RolesDto roles);
    }
}
