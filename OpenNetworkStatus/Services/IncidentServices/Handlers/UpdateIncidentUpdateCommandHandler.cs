using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.IncidentServices.Commands;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class UpdateIncidentUpdateCommandHandler : IRequestHandler<UpdateIncidentUpdateCommand, IncidentUpdateResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public UpdateIncidentUpdateCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IncidentUpdateResource> Handle(UpdateIncidentUpdateCommand request, CancellationToken cancellationToken)
        {
            var update = new IncidentUpdate
            {
                Id = request.Id,
                Status = request.Status,
                Message = request.Message
            };

            _dataContext.Attach(update);
            _dataContext.Entry(update).Property(x => x.Status).IsModified = true;
            _dataContext.Entry(update).Property(x => x.Message).IsModified = true;

            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!IncidentUpdateExists(request.Id))
            {
                return null;
            }
            
            return IncidentUpdateResource.FromIncidentUpdate(update);
        }
        
        private bool IncidentUpdateExists(int id) => _dataContext.IncidentUpdates.Any(e => e.Id == id);
    }
}