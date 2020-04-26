using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.IncidentServices.Commands;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class UpdateIncidentCommandHandler : IRequestHandler<UpdateIncidentCommand, IncidentResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public UpdateIncidentCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IncidentResource> Handle(UpdateIncidentCommand request, CancellationToken cancellationToken)
        {
            var incident = new Incident
            {
                Id = request.Id,
                Impact = request.Impact,
                Title = request.Title,
                ResolvedAt = request.ResolvedAt
            };

            _dataContext.Entry(incident).State = EntityState.Modified;
            
            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!IncidentExists(request.Id))
            {
                return null;
            }
            
            return IncidentResource.FromIncident(incident);
        }
        
        private bool IncidentExists(int id) => _dataContext.Incidents.Any(e => e.Id == id);
    }
}