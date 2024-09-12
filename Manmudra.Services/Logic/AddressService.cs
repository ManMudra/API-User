using AutoMapper;
using Manmudra.Contract;
using Manmudra.Data.Context;
using Manmudra.Data.Entities;
using Manmudra.DTO.Address;
using Manmudra.DTO.Response;
using Manmudra.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Manmudra.Services.Logic
{
    public class AddressService(ManmudraContext dbContext, IMapper mapper) : IAddressService
    {
        private readonly ManmudraContext dbContext = dbContext;
        private readonly IMapper mapper = mapper;

        public async Task<ApiResponse<AddressDto>> Get(int skipCount, string userId)
        {
            var response = new ApiResponse<AddressDto>();
            try
            {
                Expression<Func<Address, bool>> where = x =>
                (x.StateId != null || x.DistrictId != null || x.SubDistrictId != null || x.BlockId != null || x.PanchayatId != null || !string.IsNullOrEmpty(x.CityOrVillage) || x.PinCode != null || x.HouseNo != null || x.PostOffice != null) &&
                ((x.LastUpdatedBy != null && x.LastUpdatedBy == userId) || (x.CreatedBy != null && x.CreatedBy == userId));
                var trackedAddress = await dbContext.Addresses.Where(where).OrderBy(x => x.LastUpdateDate ?? x.CreationDate).FirstOrDefaultAsync();
                response.Data = this.mapper.Map<AddressDto>(trackedAddress);
                response.IsSuccess = true;
                response.Message = Messages.Success.Fetched;
            }
            catch (Exception ex)
            {
                response.Message = $"Exception: {ex.Message}, {Environment.NewLine} InnerException: {ex.InnerException}";
            }
            return response;
        }
    }
}
