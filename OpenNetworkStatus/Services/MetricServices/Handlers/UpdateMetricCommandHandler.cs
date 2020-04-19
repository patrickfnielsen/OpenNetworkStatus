using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.MetricServices.Commands;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class UpdateMetricCommandHandler : IRequestHandler<UpdateMetricCommand, MetricResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public UpdateMetricCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<MetricResource> Handle(UpdateMetricCommand request, CancellationToken cancellationToken)
        {
            var metric = new Metric
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Suffix = request.Suffix,
                Position = request.Position
            };
            
            _dataContext.Entry(metric).State = EntityState.Modified;
            
            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!MetricExists(request.Id))
            {
                return null;
            }
            
            return MetricResource.FromMetric(metric);
        }
        
        private bool MetricExists(int id) => _dataContext.Metrics.Any(e => e.Id == id);
    }
}