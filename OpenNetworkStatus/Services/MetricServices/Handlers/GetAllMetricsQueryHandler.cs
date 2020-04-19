using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.ComponentServices.Resources;
using OpenNetworkStatus.Services.MetricServices.Queries;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class GetAllMetricsQueryHandler : IRequestHandler<GetAllMetricsQuery, List<MetricResource>>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetAllMetricsQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<MetricResource>> Handle(GetAllMetricsQuery request, CancellationToken cancellationToken)
        {
            var metrics = await _dataContext.Metrics
                .Page(request.Page, request.Limit)
                .MetricOrder()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return metrics.Select(MetricResource.FromMetric).ToList();
        }
    }
}