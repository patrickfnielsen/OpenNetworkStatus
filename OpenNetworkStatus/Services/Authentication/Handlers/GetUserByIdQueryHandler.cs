using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.Authentication.Queries;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.Authentication.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResource>
    {
        private readonly StatusDataContext _dataContext;

        public GetUserByIdQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<UserResource> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _dataContext.Users.FindAsync(request.Id);
            if (user == null)
            {
                return null;
            }

            return UserResource.FromUser(user);
        }
    }
}
