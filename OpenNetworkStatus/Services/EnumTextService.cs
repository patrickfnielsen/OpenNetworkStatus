using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Models.Enums;

namespace OpenNetworkStatus.Services
{
    public static class EnumTextService
    {
        public static string GetSiteStatus(SiteStatus status)
        {
            var statusText = status switch
            {
                SiteStatus.AllOperational => "All Systems Operational",
                SiteStatus.SystemPerformanceImpacted => "Some systems are experiencing performance issues",
                SiteStatus.PartialSystemIssues => "Some systems are experiencing issues",
                SiteStatus.MajorSystemIssues => "Some systems are experiencing a major outage",
                _ => "Unknown system status"
            };
            
            return statusText;
        }

        public static string GetSiteStatusIcon(SiteStatus status)
        {
            var statusText = status switch
            {
                SiteStatus.AllOperational => "favicon-operational.ico",
                SiteStatus.SystemPerformanceImpacted => "favicon-performance.ico",
                SiteStatus.PartialSystemIssues => "favicon-partial.ico",
                SiteStatus.MajorSystemIssues => "favicon-major.ico",
                _ => null
            };

            return statusText;
        }

        public static string GetComponentStatus(ComponentStatus status)
        {
            var statusText= status switch
            {
                ComponentStatus.Operational          => "Operational",
                ComponentStatus.PerformanceIssues    => "Performance Issues",
                ComponentStatus.PartialOutage        => "Partial Outage",
                ComponentStatus.MajorOutage          => "Major Outage",
                _                                    => "Unknown"
            };

            return statusText;
        }
        
        public static string GetIncidentStatus(IncidentStatus status)
        {
            var statusText= status switch
            {
                IncidentStatus.Investigating         => "Investigating",
                IncidentStatus.Identified            => "Identified",
                IncidentStatus.Monitoring            => "Monitoring",
                IncidentStatus.Update                => "Update",
                IncidentStatus.Resolved              => "Resolved",
                _                                    => "Unknown"
            };

            return statusText;
        }
    }
}