using System.Collections.Generic;

namespace OpenNetworkStatus.Models.Options
{
    public class SiteOptions
    {
        public string Name { get; set; } = "OpenNetworkStatus";

        public bool EnableSwagger { get; set; } = false;

        public bool TwoColumns { get; set; } = false;
        
        public CorsOptions Cors { get; set; }
        
        public JwtOptions Jwt { get; set; }
    }
}