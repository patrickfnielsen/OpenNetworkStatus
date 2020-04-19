using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.ComponentGroupServices.Commands;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Handlers
{
    public class AddComponentGroupCommandHandler : IRequestHandler<AddComponentGroupCommand, ComponentGroupResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public AddComponentGroupCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ComponentGroupResource> Handle(AddComponentGroupCommand request, CancellationToken cancellationToken)
        {
            var group = new ComponentGroup()
            {
                Name = request.Title,
                Position = request.Position,
                ExpandOption = request.ExpandOption,
            };
            
            _dataContext.ComponentGroups.Add(group);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return ComponentGroupResource.FromComponentGroup(group);
        }
    }
}