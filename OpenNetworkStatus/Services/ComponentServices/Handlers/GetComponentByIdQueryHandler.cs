using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.ComponentServices.Commands;
using OpenNetworkStatus.Services.ComponentServices.Queries;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentServices.Handlers
{
    public class GetComponentByIdQueryHandler : IRequestHandler<GetComponentByIdQuery, ComponentResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetComponentByIdQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ComponentResource> Handle(GetComponentByIdQuery request, CancellationToken cancellationToken)
        {
            var component = await _dataContext.Components.FindAsync(request.Id);
            if (component == null)
            {
                return null;
            }

            return ComponentResource.FromComponent(component);
        }
    }
}