using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services
{
    public static class StatusTextService
    {
        public static string GetSiteStatus(SiteStatus status)
        {
            var statusText = status switch
            {
                SiteStatus.AllOperational            => "All Systems Operational",
                SiteStatus.SystemPerformanceImpacted => "Some systems are experiencing performance issues",
                SiteStatus.PartialSystemIssues       => "Some systems are experiencing issues",
                SiteStatus.MajorSystemIssues         => "Some systems are experiencing a major outage",
                _                                    => "Unknown system status"
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
                IncidentStatus.Resolved              => "Resolved",
                _                                    => "Unknown"
            };

            return statusText;
        }
    }
}