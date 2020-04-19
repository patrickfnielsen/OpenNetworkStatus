namespace OpenNetworkStatus.Models.Options
{
    public class SiteOptions
    {
        public string Name { get; set; } = "OpenNetworkStatus";
        
        public CorsOptions Cors { get; set; }
    }
}