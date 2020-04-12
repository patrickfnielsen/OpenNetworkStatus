using System.Collections.Generic;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services;
using OpenNetworkStatus.Services.StatusServices.ViewModels;

namespace OpenNetworkStatus.Models
{
    public struct StatusViewModel
    {
        public StatusViewModel(SiteStatus status, IEnumerable<ComponentGroup> componentGroups, IEnumerable<Component> components, IEnumerable<Metric> metrics, IEnumerable<StatusDayViewModel> statusDays)
        {
            Status = status;
            ComponentGroups = componentGroups;
            Components = components;
            Metrics = metrics;
            StatusDays = statusDays;
        }

        public string SiteStatusText => StatusTextService.GetSiteStatus(Status);

        public SiteStatus Status { get; }
        
        public IEnumerable<ComponentGroup> ComponentGroups { get; }

        public IEnumerable<Component> Components { get; }
        
        public IEnumerable<Metric> Metrics { get; }
        
        public IEnumerable<StatusDayViewModel> StatusDays { get; }
    }
}