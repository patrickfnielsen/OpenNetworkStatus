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
    public class GetIncidentUpdatesForIncidentQueryHandler : IRequestHandler<GetIncidentUpdatesForIncidentQuery, List<IncidentUpdateResource>>
    {
        private readonly StatusDataContext _dataContext;

        public GetIncidentUpdatesForIncidentQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<IncidentUpdateResource>> Handle(GetIncidentUpdatesForIncidentQuery request, CancellationToken cancellationToken)
        {
            var updates = await _dataContext.IncidentUpdates
                .Where(x => x.IncidentId == request.IncidentId)
                .Page(request.Page, request.Limit)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return updates.Select(IncidentUpdateResource.FromIncidentUpdate).ToList();
        }
    }
}