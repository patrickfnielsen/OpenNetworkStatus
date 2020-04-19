using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.ComponentServices.Queries;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentServices.Handlers
{
    public class GetAllComponentsQueryHandler : IRequestHandler<GetAllComponentsQuery, List<ComponentResource>>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetAllComponentsQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<ComponentResource>> Handle(GetAllComponentsQuery request, CancellationToken cancellationToken)
        {
            var components = await _dataContext.Components
                .Page(request.Page, request.Limit)
                .ComponentOrder()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return components.Select(ComponentResource.FromComponent).ToList();
        }
    }
}