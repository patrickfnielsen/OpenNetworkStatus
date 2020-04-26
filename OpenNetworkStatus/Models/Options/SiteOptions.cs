using System.Collections.Generic;

namespace OpenNetworkStatus.Models.Options
{
    public class SiteOptions
    {
        public string Name { get; set; } = "OpenNetworkStatus";
        
        public CorsOptions Cors { get; set; }
        
        public JwtOptions Jwt { get; set; }
    }
}