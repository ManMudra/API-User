using Manmudra.Contract.Settings;
using Manmudra.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace Manmudra.Services.Logic
{
    public class ExceptionService : IExceptionService
    {
        private readonly AppSettings appSettings;
        public ExceptionService(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public async Task SendExceptionEmail(Exception exception, HttpContext httpContext, string action = "", string controller = "", bool exceptionText = false, string? message = null, string? requestBody = null)
        {
            string ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
            string baseUrl = appSettings.BaseUrl;
            var userId = string.Empty;
            var userPhone = string.Empty;
            var user = httpContext.User;
            if (string.IsNullOrEmpty(requestBody))
            {
                requestBody = "";
            }
            if (string.IsNullOrEmpty(action))
            {
                action = httpContext.Request.Path.ToString();
            }
            if (user != null)
            {
                userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                userPhone = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.MobilePhone)?.Value;
            }
            var htmlString = $@"
                    <h3>Error occured</h3>
                    <p>Environment API Url: {baseUrl}</p>
                    <p>IP Address: {ipAddress}</p>
                    <p>Controller: {controller}</p>
                    <p>Action: {action}</p>
                    <p>API: {httpContext.Request.Path.ToString()}</p>
                    <p>User Token: {httpContext.Request.Headers["Authorization"]}</p>
                    <p>User Id: {userId}</p>
                    <p>User Phone Number: {userPhone}</p>
                    <p>Request Body: {requestBody}</p>
                    <p>Error: {(exceptionText ? message : exception.ToString())}</p>
                ";
            string[] toEmails = new string[] { "shaikhij2000@gmail.com", "ashimjain@gmail.com" };
            await SendEmail($"Error occured in API at {DateTime.Now.ToString("yyyy-MM-dd hh:mm tt")}", htmlString, string.Join(",", toEmails));
        }

        private async Task<bool> SendEmail(string subject, string message, string? toEmail = null)
        {
            try
            {
                if (string.IsNullOrEmpty(toEmail))
                {
                    toEmail = appSettings.FromEmail;
                }
                var smtp = "smtp.zoho.com";
                var port = 587;
                using (SmtpClient client = new SmtpClient(smtp, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(appSettings.FromEmail, appSettings.FromEmailPassword);
                    MailMessage mm = new MailMessage(appSettings.FromEmail, toEmail, subject, message);
                    mm.IsBodyHtml = true;
                    await client.SendMailAsync(mm);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
