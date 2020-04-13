using System.Collections.Generic;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Models
{
    public struct StatusViewModel
    {
        public StatusViewModel(SiteStatusViewModel siteStatus, IEnumerable<ComponentGroup> componentGroups, IEnumerable<Component> components, IEnumerable<Metric> metrics, IEnumerable<StatusDayViewModel> statusDays)
        {
            SiteStatus = siteStatus;
            ComponentGroups = componentGroups;
            Components = components;
            Metrics = metrics;
            StatusDays = statusDays;
        }

        public SiteStatusViewModel SiteStatus { get; }
        
        public IEnumerable<ComponentGroup> ComponentGroups { get; }

        public IEnumerable<Component> Components { get; }
        
        public IEnumerable<Metric> Metrics { get; }
        
        public IEnumerable<StatusDayViewModel> StatusDays { get; }
    }
}