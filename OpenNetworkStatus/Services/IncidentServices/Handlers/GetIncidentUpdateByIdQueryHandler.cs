using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.IncidentServices.Queries;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, IncidentResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetIncidentByIdQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IncidentResource> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
        {
            var incident = await _dataContext.Incidents
                .Include(x => x.Updates)
                .SingleOrDefaultAsync(x => x.Id == request.IncidentId);

            if (incident == null)
            {
                return null;
            }

            return IncidentResource.FromIncident(incident);
        }
    }
}