using Microsoft.AspNetCore.Http;

namespace Manmudra.Services.Interface
{
    public interface IExceptionService
    {
        Task SendExceptionEmail(Exception exception, HttpContext httpContext, string action = "", string controller = "", bool exceptionText = false, string? message = null, string? requestBody = null);
    }
}
