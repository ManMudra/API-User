using Manmudra.DTO.Exceptions;
using Manmudra.Services.Interface;
using Manmudra.Services.Logic;

namespace Manmudra.GroupsAPI.Extensions
{
    public static class DependencyCollectionExtension
    {
        public static void InjectDependency(this IServiceCollection services)
        {
            services.AddScoped<IExceptionResponse, ExceptionResponse>();
            services.AddScoped<IExceptionService, ExceptionService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
