using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.IncidentServices.Queries;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class GetIncidentUpdateByIdQueryHandler : IRequestHandler<GetIncidentUpdateByIdQuery, IncidentUpdateResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetIncidentUpdateByIdQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IncidentUpdateResource> Handle(GetIncidentUpdateByIdQuery request, CancellationToken cancellationToken)
        {
            var update = await _dataContext.IncidentUpdates.FindAsync(request.UpdateId);
            if (update == null)
            {
                return null;
            }

            return IncidentUpdateResource.FromIncidentUpdate(update);
        }
    }
}