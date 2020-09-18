using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.ComponentGroupServices.Commands;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Handlers
{
    public class UpdateComponentGroupCommandHandler : IRequestHandler<UpdateComponentGroupCommand, ComponentGroupResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public UpdateComponentGroupCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        

        public async Task<ComponentGroupResource> Handle(UpdateComponentGroupCommand request, CancellationToken cancellationToken)
        {
            var componentGroup = new ComponentGroup
            {
                Id = request.Id,
                Name = request.Title,
                Position = request.Position,
                Display = request.Display,
                ExpandOption = request.ExpandOption
            };
            
            _dataContext.Entry(componentGroup).State = EntityState.Modified;
            
            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!ComponentGroupExists(request.Id))
            {
                return null;
            }
            
            return ComponentGroupResource.FromComponentGroup(componentGroup);
        }
        
        private bool ComponentGroupExists(int id) => _dataContext.ComponentGroups.Any(e => e.Id == id);
    }
}