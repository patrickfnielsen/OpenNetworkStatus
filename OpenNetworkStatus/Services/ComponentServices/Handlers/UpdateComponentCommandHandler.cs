using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.ComponentServices.Commands;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentServices.Handlers
{
    public class UpdateComponentCommandHandler : IRequestHandler<UpdateComponentCommand, ComponentResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public UpdateComponentCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ComponentResource> Handle(UpdateComponentCommand request, CancellationToken cancellationToken)
        {
            var component = new Component
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Position = request.Position,
                ComponentGroupId = request.GroupId
            };
            
            _dataContext.Entry(component).State = EntityState.Modified;
            
            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!ComponentExists(request.Id))
            {
                return null;
            }
            
            return ComponentResource.FromComponent(component);
        }
        
        private bool ComponentExists(int id) => _dataContext.Components.Any(e => e.Id == id);
    }
}