using System.Collections.Generic;
using System.Linq;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Models.Enums;

namespace OpenNetworkStatus.Services.StatusServices
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