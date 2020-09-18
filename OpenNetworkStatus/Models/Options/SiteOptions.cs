namespace OpenNetworkStatus.Models.Options
{
    public class SiteOptions
    {
        public string Name { get; set; } = "OpenNetworkStatus";

        public bool EnableSwagger { get; set; } = false;

        public LayoutOptions Layout { get; set; } = new LayoutOptions();

        public CorsOptions Cors { get; set; } = new CorsOptions();

        public JwtOptions Jwt { get; set; } = new JwtOptions();
    }
}