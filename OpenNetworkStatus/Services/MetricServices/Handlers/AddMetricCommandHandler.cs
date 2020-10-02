using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.MetricServices.Commands;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class AddMetricCommandHandler : IRequestHandler<AddMetricCommand, MetricResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public AddMetricCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<MetricResource> Handle(AddMetricCommand request, CancellationToken cancellationToken)
        {
            var metric = new Metric
            {
                Title = request.Title,
                Description = request.Description,
                Display = request.Display,
                Suffix = request.Suffix,
                Position = request.Position,
                DecimalPlaces = request.DecimalPlaces,
                MetricProviderType = request.MetricProviderType.ToLower(),
                ExternalMetricIdentifier = request.ExternalMetricIdentifier
            };
            
            _dataContext.Metrics.Add(metric);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return MetricResource.FromMetric(metric);
        }
    }
}