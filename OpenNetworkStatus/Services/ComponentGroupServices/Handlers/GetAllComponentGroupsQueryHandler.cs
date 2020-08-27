using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.ComponentGroupServices.Commands;
using OpenNetworkStatus.Services.ComponentGroupServices.Queries;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Handlers
{
    public class GetAllComponentGroupsQueryHandler : IRequestHandler<GetAllComponentGroupsQuery, List<ComponentGroupResource>>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetAllComponentGroupsQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<ComponentGroupResource>> Handle(GetAllComponentGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = await _dataContext.ComponentGroups
                .Page(request.Page, request.Limit)
                .Include(x => x.Components)
                .ComponentGroupOrder()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return groups.Select(ComponentGroupResource.FromComponentGroup).ToList();
        }
    }
}