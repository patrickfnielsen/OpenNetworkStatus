using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.Authentication.Commands;

namespace OpenNetworkStatus.Services.ComponentServices.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly StatusDataContext _dataContext;
        
        public DeleteUserCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dataContext.Users.FindAsync(request.Id);
            if (user == null)
            {
                return false;
            }

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}