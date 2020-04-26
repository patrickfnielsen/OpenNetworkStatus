using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.IncidentServices.Queries;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class GetAllIncidentsQueryHandler : IRequestHandler<GetAllIncidentsQuery, List<IncidentResource>>
    {
        private readonly StatusDataContext _dataContext;

        public GetAllIncidentsQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<IncidentResource>> Handle(GetAllIncidentsQuery request, CancellationToken cancellationToken)
        {
            var incidents = await _dataContext.Incidents
                .Include(x => x.Updates)
                .Page(request.Page, request.Limit)
                .IncidentOrder()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return incidents.Select(IncidentResource.FromIncident).ToList();
        }
    }
}