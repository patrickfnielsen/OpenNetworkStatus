using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.Authentication.Queries;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.Authentication.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserResource>>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetAllUsersQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<UserResource>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var components = await _dataContext.Users
                .Page(request.Page, request.Limit)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return components.Select(UserResource.FromUser).ToList();
        }
    }
}