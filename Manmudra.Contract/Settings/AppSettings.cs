namespace Manmudra.Contract.Settings
{
    public class AppSettings
    {
        public string? DbTimeoutInSecond { get; set; }
        public string Environment { get; set; } = String.Empty;
        public string Secret { get; set; } = String.Empty;
        public string DatabaseString { get; set; } = String.Empty;
        public string BaseUrl { get; set; } = String.Empty;
        public string FromEmail { get; set; } = String.Empty;
        public string FromEmailPassword { get; set; } = String.Empty;
        public TimeSpan BlockedFor { get; set; }
        public bool ShowSwaggerInProd { get; set; } = false;
    }
}
