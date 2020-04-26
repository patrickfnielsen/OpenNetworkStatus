using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.IncidentServices.Commands;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class DeleteIncidentUpdateCommandHandler : IRequestHandler<DeleteIncidentUpdateCommand, bool>
    {
        private readonly StatusDataContext _dataContext;
        
        public DeleteIncidentUpdateCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteIncidentUpdateCommand request, CancellationToken cancellationToken)
        {
            var incidentUpdate = await _dataContext.IncidentUpdates.FindAsync(request.Id);
            if (incidentUpdate == null)
            {
                return false;
            }

            _dataContext.IncidentUpdates.Remove(incidentUpdate);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}