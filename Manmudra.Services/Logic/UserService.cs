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
using Newtonsoft.Json;

namespace Manmudra.Services.Logic
{
    public class UserService(UserManager<ApplicationUser> userManager, IMapper mapper, ManmudraContext dbContext) : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IMapper mapper = mapper;
        private readonly ManmudraContext dbContext = dbContext;

        private async Task<int?> ManageAddress(AddressDto address)
        {
            var existingAddress = await dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == address.Id);
            var addressId = existingAddress?.Id;
            if (existingAddress is null)
            {
                if (await dbContext.Addresses.AnyAsync())
                {
                    address.Id = dbContext.Addresses.Max(x => x.Id) + 1;
                }
                else
                {
                    address.Id = 1;
                }
                existingAddress = mapper.Map<Address>(address);
                await dbContext.Addresses.AddAsync(existingAddress);
            }
            else
            {
                mapper.Map(address, existingAddress);
                dbContext.Addresses.Update(existingAddress);
            }
            await dbContext.SaveChangesAsync();
            addressId = existingAddress.Id;
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

        public async Task<ApiResponse<UserDto>> GetUserByPhoneNumber(string phoneNumber)
        {
            var response = new ApiResponse<UserDto>();
            try
            {
                response.Data = mapper.Map<UserDto>(await dbContext.Users.Include(x => x.Address).FirstOrDefaultAsync(x => (!string.IsNullOrEmpty(x.PhoneNumber) && (x.PhoneNumber.Replace("-", "") == phoneNumber.Replace("-", "") || x.PhoneNumber == phoneNumber))));
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<UserDto>>> GetUsersByPhoneNumber(string phoneNumber)
        {
            var response = new ApiResponse<List<UserDto>>();
            try
            {
                var mobileNumberWithoutCode = phoneNumber.Replace("+91", "");
                response.Data = mapper.Map<List<UserDto>>(await dbContext.Users.Include(x => x.Address).Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && (x.PhoneNumber.Replace("-", "") == phoneNumber.Replace("-", "") ||
                    x.PhoneNumber == phoneNumber ||
                    x.PhoneNumber.Replace("-", "") == mobileNumberWithoutCode.Replace("-", "") ||
                    x.PhoneNumber == mobileNumberWithoutCode)).ToListAsync());
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

        public async Task<ApiResponse<List<UserDto>>> GetUsersByUnion(int unionId)
        {
            var response = new ApiResponse<List<UserDto>>();
            try
            {
                response.Data = mapper.Map<List<UserDto>>(await dbContext.Users.Include(x => x.Address).Where(x => x.UnionId == unionId).ToListAsync());
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<ApplicationUserRolesDto>>> GetApplicationRoleByUser(string id, int unionId)
        {
            var response = new ApiResponse<List<ApplicationUserRolesDto>>();
            try
            {
                response.Data = mapper.Map<List<ApplicationUserRolesDto>>(await dbContext.ApplicationUserRoles.Where(x => x.UnionId == unionId && x.UserId == id).ToListAsync());
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<ApplicationUserRolesDto>>> GetApplicationRoleByUserIds(IdsDto ids)
        {
            var response = new ApiResponse<List<ApplicationUserRolesDto>>();
            try
            {
                response.Data = mapper.Map<List<ApplicationUserRolesDto>>(await dbContext.ApplicationUserRoles.Where(x => ids.Id.Contains(x.UserId)).ToListAsync());
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<ApplicationUserRolesDto>>> GetApplicationRoleByUnion(int unionId)
        {
            var response = new ApiResponse<List<ApplicationUserRolesDto>>();
            try
            {
                response.Data = mapper.Map<List<ApplicationUserRolesDto>>(await dbContext.ApplicationUserRoles.Where(x => x.UnionId == unionId).ToListAsync());
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<AddressDto>>> GetAddressByIds(IdsDto ids)
        {
            var response = new ApiResponse<List<AddressDto>>();
            try
            {
                response.Data = mapper.Map<List<AddressDto>>(await dbContext.Addresses.Where(x => ids.Id.Contains(x.Id.ToString())).ToListAsync());
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

        public async Task<ApiResponse<List<UserDto>>> GetUserByIds(IdsDto ids)
        {
            var response = new ApiResponse<List<UserDto>>();
            try
            {
                response.Data = mapper.Map<List<UserDto>>(await dbContext.Users.Include(x => x.Address).Where(x => ids.Id.Contains(x.Id.ToString())).ToListAsync());
                response.IsSuccess = response.Data is not null;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<bool>> MigrateDataToServer(MigrateDto migrate)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            var response = new ApiResponse<bool>();
            try
            {
                var applicationUsers = mapper.Map<List<ApplicationUser>>(migrate.Users);

                // Step 1: Extract IDs from the incoming API data
                var incomingUserIds = applicationUsers.Select(x => x.Id).ToList();
                var incomingAddressIds = applicationUsers.Where(x => x.Address != null).Select(x => x.AddressId).ToList();
                var incomingUserRoleIds = migrate.ApplicationUserRoles.Select(x => x.Id).ToList();

                // Step 2: Query only existing records that match the incoming IDs
                var existingUsers = await dbContext.Users
                    .Where(x => incomingUserIds.Contains(x.Id))
                    .ToListAsync();

                var existingAddresses = await dbContext.Addresses
                    .Where(x => incomingAddressIds.Contains(x.Id))
                    .ToListAsync();

                var existingUserRoles = await dbContext.ApplicationUserRoles
                    .Where(x => incomingUserRoleIds.Contains(x.Id))
                    .ToListAsync();

                if (!await dbContext.Roles.AnyAsync())
                {
                    foreach (var role in migrate.Roles)
                    {
                        await dbContext.Roles.AddAsync(new IdentityRole
                        {
                            Id = role.Id,
                            Name = role.Name,
                        });
                    }
                    await dbContext.SaveChangesAsync();
                }

                // Step 3: Upsert Users
                foreach (var user in applicationUsers)
                {
                    var existingUser = existingUsers.FirstOrDefault(x => x.Id == user.Id);

                    if (existingUser != null)
                    {
                        // Update existing user
                        dbContext.Entry(existingUser).CurrentValues.SetValues(user);
                    }
                    else
                    {
                        // Insert new user
                        dbContext.Users.Add(user);
                    }
                }

                // Step 4: Upsert Addresses
                var addressesToUpsert = applicationUsers.Where(x => x.Address != null && x.AddressId > 0).ToList();
                var addressIds = new List<int>();
                foreach (var address in addressesToUpsert)
                {
                    address.Address.Id = address.AddressId ?? 0;
                    if (address.AddressId > 0)
                    {
                        if (addressIds.Contains(address.AddressId.Value))
                        {
                            continue;
                        }
                        else
                        {
                            addressIds.Add(address.AddressId.Value);
                        }
                    }
                    var existingAddress = existingAddresses.FirstOrDefault(x => x.Id == address.AddressId);

                    if (existingAddress != null)
                    {
                        // Update existing address
                        dbContext.Entry(existingAddress).CurrentValues.SetValues(address.Address);
                    }
                    else
                    {
                        // Insert new address
                        dbContext.Addresses.Add(address.Address);
                    }
                }

                // Step 5: Upsert User Roles
                var userRolesToUpsert = mapper.Map<List<ApplicationUserRoles>>(migrate.ApplicationUserRoles);
                foreach (var role in userRolesToUpsert)
                {
                    var existingRole = existingUserRoles.FirstOrDefault(x => x.Id == role.Id);

                    if (existingRole != null)
                    {
                        // Update existing role
                        dbContext.Entry(existingRole).CurrentValues.SetValues(role);
                    }
                    else
                    {
                        // Insert new role
                        dbContext.ApplicationUserRoles.Add(role);
                    }
                }

                // Step 6: Save all changes to the database
                await dbContext.SaveChangesAsync();


                await transaction.CommitAsync();
                response.IsSuccess = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<UserDto>> ValidateAadharNumber(string id, int unionId)
        {
            var response = new ApiResponse<UserDto>();
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    response.Message = "User not found";
                    return response;
                }
                if (string.IsNullOrEmpty(user.AadhaarNo?.ToString()))
                {
                    response.IsSuccess = true;
                    return response;
                }
                var findUser = await this.dbContext.Users.FirstOrDefaultAsync(x => (!string.IsNullOrEmpty(id) ? x.Id != user.Id : true) && (x.UnionId == unionId || x.AadhaarNo == user.AadhaarNo));

                if (findUser == null)
                {
                    response.IsSuccess = true;
                    response.Data = mapper.Map<UserDto>(findUser);
                    return response;
                }

                response.Message = "Aadhaar number already exist in this union";
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<string>>> SearchVillage(AddressSearchDto model)
        {
            var response = new ApiResponse<List<string>>();
            try
            {
                var villages = dbContext.Addresses.Where(x => !string.IsNullOrEmpty(x.CityOrVillage) && x.CityOrVillage.ToLower().StartsWith(model.Text.ToLower()));
                if (model.StateId > 0)
                {
                    villages = villages.Where(x => x.StateId == model.StateId);
                }
                if (model.DistrictId > 0)
                {
                    villages = villages.Where(x => x.DistrictId == model.DistrictId);
                }
                if (model.SubDistrictId > 0)
                {
                    villages = villages.Where(x => x.SubDistrictId == model.SubDistrictId);
                }
                if (model.BlockId > 0)
                {
                    villages = villages.Where(x => x.BlockId == model.BlockId);
                }
                if (model.PanchayatId > 0)
                {
                    villages = villages.Where(x => x.PanchayatId == model.PanchayatId);
                }
                response.Data = await villages.Select(x => x.CityOrVillage).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<string>>> SearchCaste(string searchText)
        {
            var response = new ApiResponse<List<string>>();
            try
            {
                response.Data = await dbContext.Users.Where(x => !string.IsNullOrEmpty(x.Caste) && x.Caste.ToLower().StartsWith(searchText.ToLower())).Select(x => x.Caste).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<int>> TotalUsersCount(long unionId, string? panchayatIds)
        {
            var response = new ApiResponse<int>();
            try
            {
                var panchayatUsers = new List<string>();
                var usersData = dbContext.Users.Include(x => x.Address).Where(x => x.UnionId == unionId);
                if (!string.IsNullOrEmpty(panchayatIds))
                {
                    var panchayatId = panchayatIds.Split(',').Select(int.Parse).ToList();
                    if (panchayatId.Count > 0)
                    {
                        usersData = usersData.Where(x => x.Address != null && x.Address.PanchayatId != null && panchayatId.Contains(x.Address.PanchayatId.Value));
                    }
                }
                response.Data = await usersData.CountAsync();
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<string>>> TotalTransactionCount(long unionId, string? panchayatIds)
        {
            var response = new ApiResponse<List<string>>();
            try
            {
                var panchayatUsers = new List<string>();
                var usersData = dbContext.Users.Include(x => x.Address).Where(x => x.UnionId == unionId);
                if (!string.IsNullOrEmpty(panchayatIds))
                {
                    var panchayatId = panchayatIds.Split(',').Select(int.Parse).ToList();
                    if (panchayatId.Count > 0)
                    {
                        usersData = usersData.Where(x => x.Address != null && x.Address.PanchayatId != null && panchayatId.Contains(x.Address.PanchayatId.Value));
                    }
                }
                response.Data = await usersData.Select(x => x.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<List<int>>> GetUnionUserStates(long unionId)
        {
            var response = new ApiResponse<List<int>>();
            try
            {
                response.Data = await dbContext.Users.Include(x => x.Address).Where(x => x.UnionId == unionId && x.Address != null && x.Address.StateId != null).Select(x => x.Address.StateId.Value).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<int>> GetMemberCountInUnion(long unionId)
        {
            var response = new ApiResponse<int>();
            try
            {
                if (await dbContext.Users.AnyAsync(x => x.UnionId == unionId))
                {
                    response.Data = await dbContext.Users.CountAsync(x => x.UnionId == unionId);
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<MigrateDataBackFromServerDto>> MigrateDataBackFromServer(int unionId)
        {
            var response = new ApiResponse<MigrateDataBackFromServerDto>();
            try
            {
                var users = await dbContext.Users.Where(x => x.UnionId == unionId).ToListAsync();
                var userIds = users.Select(x => x.Id).ToList();
                var appRoles = await dbContext.ApplicationUserRoles.Where(x => userIds.Contains(x.UserId)).ToListAsync();
                response.Data = new MigrateDataBackFromServerDto
                {
                    Users = mapper.Map<List<MigrateUserDto>>(users),
                    ApplicationUserRoles = mapper.Map<List<ApplicationUserRolesDto>>(appRoles)
                };
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }
    }
}
