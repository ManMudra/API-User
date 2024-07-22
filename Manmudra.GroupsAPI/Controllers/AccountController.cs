using Manmudra.DTO.Account;
using Manmudra.DTO.Response;
using Manmudra.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Manmudra.GroupsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(ILogger<AccountController> logger, IAccountService accountService) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IAccountService _accountService = accountService;

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponse<string>>> Register(RegisterDto register)
        {
            var response = await this._accountService.Register(register);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> LoginAsync(LoginDto login)
        {
            var response = await this._accountService.Login(login);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("SetOtpDate")]
        public async Task<ActionResult<ApiResponse<string>>> SetOtpDate(LoginDto login)
        {
            var response = await this._accountService.SetOtpDate(login);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("SendForgetPinOtp")]
        public async Task<ActionResult<ApiResponse<bool>>> SendForgetPinOtp(ForgetPinDto forgetPin)
        {
            forgetPin.MobileNo = Regex.Replace(forgetPin.MobileNo, @"[^\d]", "");
            var response = await this._accountService.SendForgetPinOtp(forgetPin);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("VerifyOTP")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyOTP(ForgetPinDto forgetPin)
        {
            forgetPin.MobileNo = Regex.Replace(forgetPin.MobileNo, @"[^\d]", "");
            var response = await this._accountService.VerifyOTP(forgetPin);
            return Ok(response);
        }
    }
}
