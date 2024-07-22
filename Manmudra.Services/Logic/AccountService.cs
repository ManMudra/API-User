using AutoMapper;
using Manmudra.Contract;
using Manmudra.Contract.Enums;
using Manmudra.Contract.Helpers;
using Manmudra.Contract.Settings;
using Manmudra.Data.Context;
using Manmudra.Data.Entities;
using Manmudra.DTO.Account;
using Manmudra.DTO.Response;
using Manmudra.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Manmudra.Services.Logic
{
    public class AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, ManmudraContext dbContext, IUserService userService, IOptions<AppSettings> appSettings) : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly SignInManager<ApplicationUser> signInManager = signInManager;
        private readonly AppSettings appSettings = appSettings.Value;
        private readonly ManmudraContext dbContext = dbContext;
        private readonly IMapper mapper = mapper;
        private readonly IUserService userService = userService;

        public async Task<ApiResponse<LoginResponseDto>> Login(LoginDto login)
        {
            var response = new ApiResponse<LoginResponseDto>();
            try
            {
                if (string.IsNullOrEmpty(login.Last4Digit))
                {
                    var countMobile = await userManager.Users.CountAsync(x => x.PhoneNumber == login.MobileNo);
                    if (countMobile > 1)
                    {
                        response.Message = "More Members";
                        return response;
                    }
                }
                var user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == login.MobileNo && (string.IsNullOrEmpty(login.Last4Digit) || (x.AadhaarNo != null && x.AadhaarNo.ToString().EndsWith(login.Last4Digit))));
                response.Message = user != null && string.IsNullOrEmpty(user.PasswordHash) ? Messages.Failure.NoPassword : "";
                if (login.CheckAadhaar == true && user != null)
                {
                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        response.Message = "Enter password";
                        response.IsSuccess = false;
                    }
                    return response;
                }
                var isMember = false;
                if (user != null) isMember = user.IsMember == null ? false : user.IsMember.Value;
                if (user == null || !isMember)
                {
                    response.Message = Messages.Failure.NoRecordFound;
                    return response;
                }
                else if (!string.IsNullOrEmpty(login.ConfirmPassword))
                {
                    if (string.IsNullOrEmpty(user.PasswordHash))
                    {
                        var result = await this.userManager.AddPasswordAsync(user, login.Password);
                        if (result.Succeeded)
                        {
                            await VerifyAsync(user);
                            response.IsSuccess = true;
                            response.Message = Messages.Success.SetPassword;
                        }
                    }
                    else
                    {
                        var resetToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await this.userManager.ResetPasswordAsync(user, resetToken, login.Password);
                        if (result.Succeeded)
                        {
                            await UnblockUser(user);
                            response.IsSuccess = true;
                            response.Message = Messages.Success.ResetPassword;
                        }
                    }
                }
                else if (DateTime.UtcNow < user.BlockedTill)
                {
                    response.Data = new LoginResponseDto
                    {
                        BlockedUser = true
                    };
                    response.Message = Messages.Failure.AttemptsExpired;
                    response.Entries = Convert.ToInt32((user.BlockedTill - DateTime.UtcNow).TotalMinutes);
                    return response;
                }
                else if (string.IsNullOrEmpty(login.Password))
                {
                    return response;
                }
                else if (DateTime.UtcNow > user.BlockedTill && user.BlockedTill != DateTime.MinValue)
                {
                    await UnblockUser(user);
                }
                var isValidPassword = await this.userManager.CheckPasswordAsync(user, login.Password);
                response.Message = Messages.Failure.InvalidCredentials;
                if (isValidPassword)
                {
                    bool verifyUnionTimeout = true;
                    bool addToOtherUnion = false;
                    if (user.UnionId > 0)
                    {
                        var roles = await dbContext.ApplicationUserRoles.Where(x => x.UserId == user.Id && x.UnionId == user.UnionId).ToListAsync();
                        if (!roles.Any())
                        {
                            addToOtherUnion = true;
                            await userService.AddUserToUnionRole(new RolesDto
                            {
                                UnionId = user.UnionId.Value,
                                UserId = user.Id,
                                Roles = [Role.OrdinaryMember.ToString()]
                            });
                        }
                        else
                        {
                            var ordinaryRole = dbContext.Roles.FirstOrDefault(x => x.Name.ToLower() == Role.OrdinaryMember.ToString().ToLower())?.Id;
                            if (!string.IsNullOrEmpty(ordinaryRole))
                            {
                                var originalRoleData = roles.FirstOrDefault(x => x.RoleId == ordinaryRole);
                                if (originalRoleData != null && roles.Count() > 1)
                                {
                                    dbContext.ApplicationUserRoles.Remove(originalRoleData);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    var authResult = await Authenticate(user);
                    if (authResult.IsSuccess)
                    {
                        await UnblockUser(user);

                        response.IsSuccess = true;
                        response.Message = Messages.Success.LoggedIn;
                    }
                    response.Data = mapper.Map<LoginResponseDto>(authResult);
                    response.Data.AddToOtherUnion = addToOtherUnion;
                    response.Data.VerifyUnionTimeout = verifyUnionTimeout;
                    response.Data.IsSuperAdmin = authResult.User.Roles.Any(x => x == Role.SuperAdmin.ToString());
                }
                else
                {
                    user.WrongAttempts++;
                    if (user.WrongAttempts > 4)
                    {
                        user.BlockedTill = DateTime.UtcNow.Add(appSettings.BlockedFor);
                        response.Message = Messages.Failure.AttemptsExpired;
                        response.Entries = Convert.ToInt32((user.BlockedTill - DateTime.UtcNow).TotalMinutes);
                    }
                    var updatedResult = await this.userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        private async Task<AuthenticationDto> Authenticate(ApplicationUser user)
        {
            var authenticationResult = new AuthenticationDto
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var userRoleIds = await dbContext.ApplicationUserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).ToListAsync();
                var userRoles = await dbContext.Roles.Where(x => userRoleIds.Contains(x.Id)).Select(x => x.Name).ToListAsync();
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.MobilePhone, (user.PhoneNumber ?? "")),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(ClaimTypes.Role, string.Join(",", userRoles))
                };

                var tokenExp = new TimeSpan(0, 30, 0);
                var refreshTokenExp = TimeSpan.FromSeconds(0).Add(tokenExp);
                var JwtSecret = Encoding.UTF8.GetBytes(appSettings.Secret);
                var authSigningKey = new SymmetricSecurityKey(JwtSecret);

                var token = new JwtSecurityToken(
                    expires: DateTime.UtcNow.Add(tokenExp),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var result = await userService.GetUserById(user.Id);

                authenticationResult.User = result?.Data;
                authenticationResult.IsSuccess = true;
                var userLogin = dbContext.UserLogins.FirstOrDefault(x => x.UserId == user.Id && x.Date == DateTime.UtcNow.Date);
                if (userLogin == null)
                {
                    userLogin = new UserLogin
                    {
                        Date = DateTime.UtcNow.Date,
                        UserId = user.Id,
                        Count = 1
                    };
                    dbContext.UserLogins.Add(userLogin);
                }
                else
                {
                    userLogin.Count = userLogin.Count + 1;
                }
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            return authenticationResult;
        }

        private async Task<bool> UnblockUser(ApplicationUser user)
        {
            user.WrongAttempts = 0;
            user.BlockedTill = DateTime.MinValue;
            var updatedResult = await this.userManager.UpdateAsync(user);
            return updatedResult.Succeeded;
        }

        private async Task<bool> VerifyAsync(ApplicationUser user)
        {
            user.VerificationDate = DateTime.UtcNow;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<ApiResponse<string>> Register(RegisterDto register)
        {
            var response = new ApiResponse<string>();
            try
            {
                var user = this.mapper.Map<ApplicationUser>(register);

                user.UserName = Guid.NewGuid().ToString();

                user.CreationDate = user.CreationDate.ConvertToUtc();
                user.BlockedTill = user.BlockedTill.ConvertToUtc();
                user.VerificationDate = user.VerificationDate.ConvertToUtc();
                user.LastUpdateDate = user.LastUpdateDate.ConvertToUtc();

                var result = await this.userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    response.IsSuccess = true;
                    response.Message = Messages.Success.UserRegistered;
                    return response;
                }

                var errors = new StringBuilder();

                foreach (var error in result.Errors)
                {
                    errors.Append($"{error.Description}{Environment.NewLine}");
                }

                response.Message = errors.ToString();
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<bool>> SendForgetPinOtp(ForgetPinDto forgetPin)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> SetOtpDate(LoginDto login)
        {
            var response = new ApiResponse<string>();
            try
            {
                var user = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == login.MobileNo);
                if (user == null) return response;
                var verified = await VerifyAsync(user);
                if (verified)
                {
                    response.IsSuccess = true;
                    response.Message = Messages.Success.Updated;
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }

        public async Task<ApiResponse<bool>> VerifyOTP(ForgetPinDto forgetPin)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (forgetPin.UpdateVerification == true)
                {
                    var user = await dbContext.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.Replace("+", "") == forgetPin.MobileNo);
                    if (user != null)
                    {
                        user.VerificationDate = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();
                    }
                }
                response.IsSuccess = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }
    }
}
