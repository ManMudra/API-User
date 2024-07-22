using Manmudra.Contract.Settings;
using Microsoft.Extensions.Options;

namespace Manmudra.GroupsAPI.Extensions
{
    public static class AppCollectionExtension
    {
        public static void InjectConfiguration(this WebApplication app, IEndpointRouteBuilder endpoints)
        {
            var appSettings = app.Services?.GetService<IOptions<AppSettings>>()?.Value;
            if (app.Environment.IsDevelopment() || appSettings.ShowSwaggerInProd)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Manmudra Groups API");
                });
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller}/{action}/{id?}");
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
