using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Manmudra.DTO.Response;
using Manmudra.DTO.Exceptions;

namespace Manmudra.GroupsAPI.Filters
{
    /// <summary>
    /// Default exceptions handler.
    /// </summary>
    /// <seealso cref="ExceptionFilterAttribute" />
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="exceptionContext">The context for the action.</param>
        public override void OnException(ExceptionContext exceptionContext)
        {
            try
            {
                var configuration = exceptionContext.HttpContext.RequestServices.GetService<IConfiguration>();
                // send email
            }
            catch
            {

            }
            exceptionContext.Result = new ObjectResult(new ApiResponse<object>
            {
                IsSuccess = false,
                ExceptionResponse = new ExceptionResponse
                {
                    ErrorMessage = exceptionContext.Exception?.Message,
                    StackTrace = exceptionContext.Exception?.StackTrace,
                    InnerExceptionMessage = exceptionContext.Exception?.ToString()
                }
            })
            { StatusCode = 200 };
        }
    }
}
