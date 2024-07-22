using Manmudra.DTO.Address;
using Manmudra.DTO.Response;
using Manmudra.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Manmudra.GroupsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController(IAddressService addressService) : ControllerBase
    {
        private readonly IAddressService addressService = addressService;

        [HttpGet("GetBySkipCount")]
        public async Task<ActionResult<ApiResponse<AddressDto>>> GetAsync(int skipCount, string userId)
        {
            var response = await this.addressService.Get(skipCount, userId);
            return Ok(response);
        }
    }
}
