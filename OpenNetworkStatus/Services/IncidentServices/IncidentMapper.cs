using System.Collections.Generic;
using System.Linq;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices
{
    public class IncidentMapper
    {
        public GetIncidentResource ToIncidentResource(Incident incident)
        {
            if (incident == null)
            {
                return null;
            }
            
            return new GetIncidentResource
            {
                Id = incident.Id,
                Impact = incident.Impact,
                Title = incident.Title,
                Updates = ToIncidentUpdateResource(incident.Updates),
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt,
                ResolvedAt = incident.ResolvedAt
            };
        }

        public List<GetIncidentResource> ToIncidentResource(List<Incident> incidents)
        {
            if (incidents == null)
            {
                return new List<GetIncidentResource>();
            }
            
            return incidents.Select(ToIncidentResource).ToList();
        }
        
        public GetIncidentUpdateResource ToIncidentUpdateResource(IncidentUpdate update)
        {
            if (update == null)
            {
                return null;
            }
            
            return new GetIncidentUpdateResource
            {
                Id = update.Id,
                IncidentId = update.IncidentId,
                Status = update.Status,
                StatusText = EnumTextService.GetIncidentStatus(update.Status),
                Message = update.Message,
                CreatedAt = update.CreatedAt,
                UpdatedAt = update.UpdatedAt
            };
        }
        
        public List<GetIncidentUpdateResource> ToIncidentUpdateResource(List<IncidentUpdate> incidentUpdates)
        {
            if (incidentUpdates == null)
            {
                return new List<GetIncidentUpdateResource>();    
            }
            
            return incidentUpdates.Select(ToIncidentUpdateResource).ToList();
        }
    }
}