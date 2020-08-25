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
    public class UpdateComponentStatusCommandHandler : IRequestHandler<UpdateComponentStatusCommand, ComponentResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public UpdateComponentStatusCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ComponentResource> Handle(UpdateComponentStatusCommand request, CancellationToken cancellationToken)
        {
            var component = new Component
            {
                Id = request.Id,
                Status = request.Status
            };

            _dataContext.Components.Attach(component);
            _dataContext.Entry(component).Property(x => x.Status).IsModified = true;
            
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