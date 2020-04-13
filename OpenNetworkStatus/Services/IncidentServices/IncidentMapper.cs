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
                Title = incident.Title,
                Updates = ToIncidentUpdateResource(incident.Updates),
                CreatedOn = incident.CreatedOn,
                UpdatedOn = incident.UpdatedOn,
                ResolvedOn = incident.ResolvedOn
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
                CreatedOn = update.CreatedOn,
                UpdatedOn = update.UpdatedOn
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