using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.IncidentServices.Commands;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class AddIncidentCommandHandler : IRequestHandler<AddIncidentCommand, IncidentResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public AddIncidentCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IncidentResource> Handle(AddIncidentCommand request, CancellationToken cancellationToken)
        {
            var incident = new Incident
            {
                Title = request.Title,
                Impact = request.Impact,
                ResolvedAt = request.ResolvedAt
            };

            _dataContext.Incidents.Add(incident);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return IncidentResource.FromIncident(incident);
        }
    }
}