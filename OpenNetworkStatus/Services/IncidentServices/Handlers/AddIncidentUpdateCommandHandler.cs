using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.IncidentServices.Commands;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class AddIncidentUpdateCommandHandler : IRequestHandler<AddIncidentUpdateCommand, IncidentUpdateResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public AddIncidentUpdateCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IncidentUpdateResource> Handle(AddIncidentUpdateCommand request, CancellationToken cancellationToken)
        {
            var incident = await _dataContext.Incidents.FindAsync(request.IncidentId);
            if (incident == null)
            {
                return null;
            }

            //If the update says the issue is resolved, update the resolved date on the incident
            if (request.Status == IncidentStatus.Resolved)
            {
                incident.ResolvedAt = DateTime.UtcNow;
            }

            //If the incidents resolved date is set, but the current sate is not resolved (ie if we backout because the issue is not really resolved)
            //The set the resolve date to null
            if (incident.ResolvedAt != null && request.Status != IncidentStatus.Resolved)
            {
                incident.ResolvedAt = null;
            }

            incident.AddUpdate(request.Status, request.Message, _dataContext);
            _dataContext.Incidents.Update(incident);
            
            await _dataContext.SaveChangesAsync(cancellationToken);

            return IncidentUpdateResource.FromIncidentUpdate(incident.Updates.First());
        }
    }
}