using System.Collections.Generic;
using System.Linq;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services
{
    public class SiteStatusCalculationService
    {
        public SiteStatus CalculateSiteStatus(List<Component> components)
        {
            if (components.Any(x => x.Status == ComponentStatus.PerformanceIssues))
            {
                return SiteStatus.SystemPerformanceImpacted;
            }
            
            if (components.Any(x => x.Status == ComponentStatus.PartialOutage))
            {
                return SiteStatus.PartialSystemIssues;
            }

            if (components.Any(x => x.Status == ComponentStatus.MajorOutage))
            {
                return SiteStatus.MajorSystemIssues;
            }

            return SiteStatus.AllOperational;
        }
    }
}