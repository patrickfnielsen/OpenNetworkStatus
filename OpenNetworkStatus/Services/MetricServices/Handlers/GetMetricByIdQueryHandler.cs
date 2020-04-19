using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.MetricServices.Queries;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class GetMetricByIdQueryHandler : IRequestHandler<GetMetricByIdQuery, MetricResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetMetricByIdQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<MetricResource> Handle(GetMetricByIdQuery request, CancellationToken cancellationToken)
        {
            var metric = await _dataContext.Metrics.FindAsync(request.Id);
            if (metric == null)
            {
                return null;
            }

            return MetricResource.FromMetric(metric);
        }
    }
}