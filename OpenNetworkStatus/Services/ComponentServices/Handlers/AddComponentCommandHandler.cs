using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.ComponentServices.Commands;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentServices.Handlers
{
    public class AddComponentCommandHandler : IRequestHandler<AddComponentCommand, ComponentResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public AddComponentCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ComponentResource> Handle(AddComponentCommand request, CancellationToken cancellationToken)
        {
            var component = new Component
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Position = request.Position,
                ComponentGroupId = request.GroupId
            };
            
            _dataContext.Components.Add(component);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return ComponentResource.FromComponent(component);
        }
    }
}