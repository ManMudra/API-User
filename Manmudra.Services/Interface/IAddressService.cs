using Manmudra.DTO.Address;
using Manmudra.DTO.Response;

namespace Manmudra.Services.Interface
{
    public interface IAddressService
    {
        Task<ApiResponse<AddressDto>> Get(int skipCount, string userId);
    }
}
