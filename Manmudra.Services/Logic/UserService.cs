using AutoMapper;
using Manmudra.Contract;
using Manmudra.Data.Context;
using Manmudra.Data.Entities;
using Manmudra.DTO.Account;
using Manmudra.DTO.Address;
using Manmudra.DTO.Response;
using Manmudra.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Manmudra.Services.Logic
{
    public class UserService(UserManager<ApplicationUser> userManager, IMapper mapper, ManmudraContext dbContext) : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IMapper mapper = mapper;
        private readonly ManmudraContext dbContext = dbContext;

        private async Task<int?> ManageAddress(AddressDto address)
        {
            var existingAddress = await dbContext.Addresses.FirstOrDefaultAsync(x => x.StateId == address.StateId && x.DistrictId == address.DistrictId && x.SubDistrictId == address.SubDistrictId && x.BlockId == address.BlockId && x.PanchayatId == address.PanchayatId && x.CityOrVillage == address.CityOrVillage && x.PinCode == address.PinCode && x.HouseNo == address.HouseNo && x.LandMark == address.LandMark && x.PostOffice == address.PostOffice);
            var addressId = existingAddress?.Id;
            if (existingAddress is null)
            {
                var addressDetail = mapper.Map<Address>(address);
                await dbContext.Addresses.AddAsync(addressDetail);

                addressId = addressDetail.Id;
            }
            else
            {
                mapper.Map(address, existingAddress);
                dbContext.Addresses.Update(existingAddress);
            }
            await dbContext.SaveChangesAsync();

            return addressId;
        }

        public async Task<ApiResponse<UserDto>> CreateUser(UserDto user)
        {
            var response = new ApiResponse<UserDto>();
            try
            {
                var userDetail = mapper.Map<ApplicationUser>(user);
                if (user.Address is not null)
                {
                    userDetail.AddressId = await ManageAddress(user.Address);
                }

                userDetail.UserName = Guid.NewGuid().ToString();

                var result = await userManager.CreateAsync(userDetail);
                await AddUserToUnionRole(new RolesDto
                {
                    UserId = userDetail.Id,
                    UnionId = user.UnionId.Value,
                    Roles = user.Roles?.ToList()
                });
                if (result.Succeeded)
                {
                    response.IsSuccess = true;
                    response.Message = Messages.Success.Added;
                }
                else if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                {
                    response.Message = $"Mobile number is already in use";
                }
                response.Data = mapper.Map<UserDto>(userDetail);
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<bool>> AddUserToUnionRole(RolesDto roles)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (roles?.Roles is not null && roles.Roles.Count > 0)
                {
                    var existingRole = await dbContext.ApplicationUserRoles.Where(x => x.UnionId == roles.UnionId && x.UserId == roles.UserId).ToListAsync();
                    if (existingRole.Count > 0)
                    {
                        dbContext.ApplicationUserRoles.RemoveRange(existingRole);
                        await dbContext.SaveChangesAsync();
                    }
                    roles.Roles = roles.Roles.Select(x => x.ToLower()).ToList();
                    var roleIds = await dbContext.Roles.Where(x => roles.Roles.Contains(x.Name.ToLower())).Select(x => x.Id).ToListAsync();
                    await dbContext.ApplicationUserRoles.AddRangeAsync(roleIds.Select(x => new ApplicationUserRoles
                    {
                        UnionId = roles.UnionId,
                        RoleId = x,
                        UserId = roles.UserId
                    }));
                    await dbContext.SaveChangesAsync();
                    response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteUser(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(id)) return response;
                var trackedUser = await userManager.FindByIdAsync(id);
                if (trackedUser is not null)
                {
                    var appRoles = dbContext.ApplicationUserRoles.Where(x => x.UserId == id).ToList();
                    if (appRoles.Count > 0)
                    {
                        dbContext.ApplicationUserRoles.RemoveRange(appRoles);
                    }
                    await userManager.DeleteAsync(trackedUser);
                    response.IsSuccess = true;
                    response.Message = Messages.Success.Deleted;
                }
                else
                {
                    response.Message = Messages.Failure.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<UserDto>> GetUserById(string id)
        {
            var response = new ApiResponse<UserDto>();
            try
            {
                response.Data = mapper.Map<UserDto>(await dbContext.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == id));
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<UserDto>>> GetUserListIds(UserFilterDto userFilter)
        {
            var response = new ApiResponse<List<UserDto>>();
            try
            {
                response.Data = mapper.Map<List<UserDto>>(await dbContext.Users.Include(x => x.Address).Where(x => x.UnionId == userFilter.UnionId && userFilter.UserIds.Contains(x.Id)).ToListAsync());
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<UserDto>> UpdateUser(UserDto user)
        {
            var response = new ApiResponse<UserDto>();
            try
            {
                var userDetail = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (user.Address is not null)
                {
                    userDetail.AddressId = await ManageAddress(user.Address);
                }
                var oldUserName = userDetail.UserName;
                this.mapper.Map(user, userDetail);
                userDetail.UserName = oldUserName;
                userDetail.Email = user.Email;
                var updatedUser = await this.userManager.UpdateAsync(userDetail);
                await AddUserToUnionRole(new RolesDto
                {
                    UserId = userDetail.Id,
                    UnionId = user.UnionId.Value,
                    Roles = user.Roles?.ToList()
                });
                if (updatedUser.Succeeded)
                {
                    response.IsSuccess = true;
                    response.Message = Messages.Success.Added;
                }
                else if (updatedUser.Errors.Any(x => x.Code == "DuplicateUserName"))
                {
                    response.Message = $"Mobile number is already in use";
                }
                response.Data = mapper.Map<UserDto>(userDetail);
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }
    }
}
