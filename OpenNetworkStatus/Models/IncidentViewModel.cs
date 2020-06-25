using System.Collections.Generic;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Models
{
    public class IncidentViewModel
    {
        public IncidentImpact Impact { get; set; }

        public string Title { get; set; }

        public List<IncidentUpdateResource> Updates { get; set; }

        public IncidentViewModel(IncidentImpact impact, string title, List<IncidentUpdateResource> updates)
        {
            Impact = impact;
            Title = title;
            Updates = updates;
        }
    }
}
