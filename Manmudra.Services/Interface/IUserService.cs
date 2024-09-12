using Manmudra.Data.Entities;
using Manmudra.DTO.Account;
using Manmudra.DTO.Address;
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
        Task<ApiResponse<List<UserDto>>> GetUsersByUnion(int unionId);
        Task<ApiResponse<bool>> AddUserToUnionRole(RolesDto roles);
        Task<ApiResponse<List<ApplicationUserRolesDto>>> GetApplicationRoleByUser(string id, int unionId);
        Task<ApiResponse<List<ApplicationUserRolesDto>>> GetApplicationRoleByUserIds(IdsDto ids);
        Task<ApiResponse<List<ApplicationUserRolesDto>>> GetApplicationRoleByUnion(int unionId);
        Task<ApiResponse<List<AddressDto>>> GetAddressByIds(IdsDto ids);
        Task<ApiResponse<UserDto>> GetUserByPhoneNumber(string phoneNumber);
        Task<ApiResponse<List<UserDto>>> GetUsersByPhoneNumber(string phoneNumber);
        Task<ApiResponse<bool>> MigrateDataToServer(MigrateDto migrate);
        Task<ApiResponse<List<UserDto>>> GetUserByIds(IdsDto ids);
        Task<ApiResponse<UserDto>> ValidateAadharNumber(string id, int unionId);
        Task<ApiResponse<List<string>>> SearchVillage(AddressSearchDto model);
        Task<ApiResponse<List<string>>> SearchCaste(string searchText);
        Task<ApiResponse<int>> TotalUsersCount(long unionId, string? panchayatIds);
        Task<ApiResponse<List<string>>> TotalTransactionCount(long unionId, string? panchayatIds);
        Task<ApiResponse<List<int>>> GetUnionUserStates(long unionId);
        Task<ApiResponse<int>> GetMemberCountInUnion(long unionId);
        Task<ApiResponse<MigrateDataBackFromServerDto>> MigrateDataBackFromServer(int unionId);
    }
}
