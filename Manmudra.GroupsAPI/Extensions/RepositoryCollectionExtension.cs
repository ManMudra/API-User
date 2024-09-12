using Manmudra.Contract.Settings;
using Manmudra.Data.Context;
using Manmudra.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Manmudra.GroupsAPI.Extensions
{
    public static class RepositoryCollectionExtension
    {
        public static void InjectRepository(this IServiceCollection services)
        {
            var appSettings = services.BuildServiceProvider()?.GetService<IOptions<AppSettings>>()?.Value;
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            services.AddDbContext<ManmudraContext>(options => options.UseNpgsql(appSettings.DatabaseString));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ManmudraContext>().AddDefaultTokenProviders();
            var builder = services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.Password.RequiredLength = 4;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<ManmudraContext>().AddDefaultTokenProviders();
        }
    }
}
