using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.ComponentServices.Commands;

namespace OpenNetworkStatus.Services.ComponentServices.Handlers
{
    public class DeleteComponentCommandHandler : IRequestHandler<DeleteComponentCommand, bool>
    {
        private readonly StatusDataContext _dataContext;
        
        public DeleteComponentCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteComponentCommand request, CancellationToken cancellationToken)
        {
            var component = await _dataContext.Components.FindAsync(request.Id);
            if (component == null)
            {
                return false;
            }

            _dataContext.Components.Remove(component);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}