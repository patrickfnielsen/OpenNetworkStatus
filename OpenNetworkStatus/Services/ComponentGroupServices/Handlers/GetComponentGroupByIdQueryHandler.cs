using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.ComponentGroupServices.Commands;
using OpenNetworkStatus.Services.ComponentGroupServices.Queries;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Handlers
{
    public class GetComponentGroupByIdQueryHandler : IRequestHandler<GetComponentGroupByIdQuery, ComponentGroupResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetComponentGroupByIdQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ComponentGroupResource> Handle(GetComponentGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var group = await _dataContext.ComponentGroups.FindAsync(request.Id);
            if (group == null)
            {
                return null;
            }

            return ComponentGroupResource.FromComponentGroup(group);
        }
    }
}