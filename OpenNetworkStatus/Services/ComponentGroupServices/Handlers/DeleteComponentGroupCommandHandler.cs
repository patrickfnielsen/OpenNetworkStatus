using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.ComponentGroupServices.Commands;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Handlers
{
    public class DeleteComponentGroupCommandHandler : IRequestHandler<DeleteComponentGroupCommand, bool>
    {
        private readonly StatusDataContext _dataContext;
        
        public DeleteComponentGroupCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteComponentGroupCommand request, CancellationToken cancellationToken)
        {
            var componentGroup = await _dataContext.ComponentGroups.FindAsync(request.Id);
            if (componentGroup == null)
            {
                return false;
            }

            _dataContext.ComponentGroups.Remove(componentGroup);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}