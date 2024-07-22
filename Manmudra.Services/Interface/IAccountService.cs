using Manmudra.DTO.Account;
using Manmudra.DTO.Response;

namespace Manmudra.Services.Interface
{
    public interface IAccountService
    {
        Task<ApiResponse<string>> Register(RegisterDto register);
        Task<ApiResponse<LoginResponseDto>> Login(LoginDto login);
        Task<ApiResponse<string>> SetOtpDate(LoginDto login);
        Task<ApiResponse<bool>> SendForgetPinOtp(ForgetPinDto forgetPin);
        Task<ApiResponse<bool>> VerifyOTP(ForgetPinDto forgetPin);
    }
}
