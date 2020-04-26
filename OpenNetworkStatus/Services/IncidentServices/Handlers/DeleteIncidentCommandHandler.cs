using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.IncidentServices.Commands;

namespace OpenNetworkStatus.Services.IncidentServices.Handlers
{
    public class DeleteIncidentCommandHandler : IRequestHandler<DeleteIncidentCommand, bool>
    {
        private readonly StatusDataContext _dataContext;
        
        public DeleteIncidentCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteIncidentCommand request, CancellationToken cancellationToken)
        {
            var incident = await _dataContext.Incidents.FindAsync(request.Id);
            if (incident == null)
            {
                return false;
            }

            _dataContext.Incidents.Remove(incident);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}