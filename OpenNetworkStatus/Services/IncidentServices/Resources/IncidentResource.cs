using System;
using System.Collections.Generic;
using System.Linq;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class IncidentResource
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public IncidentImpact Impact { get; set; }

        public List<IncidentUpdateResource> Updates { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public IncidentResource()
        {
            Updates = new List<IncidentUpdateResource>();
        }

        public static IncidentResource FromIncident(Incident incident)
        {
            var updates = incident.Updates?.Select(IncidentUpdateResource.FromIncidentUpdate).ToList() ?? new List<IncidentUpdateResource>();

            return new IncidentResource
            {
                Id = incident.Id,
                Impact = incident.Impact,
                Title = incident.Title,
                Updates = updates,
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt,
                ResolvedAt = incident.ResolvedAt
            };
        }
    }
}