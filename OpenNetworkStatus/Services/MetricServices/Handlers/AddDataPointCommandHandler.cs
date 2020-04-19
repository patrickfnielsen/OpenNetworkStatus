using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.MetricServices.Commands;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class AddDataPointCommandHandler : IRequestHandler<AddDataPointCommand, DataPointResource>
    {
        private readonly StatusDataContext _dataContext;
        
        public AddDataPointCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<DataPointResource> Handle(AddDataPointCommand request, CancellationToken cancellationToken)
        {
            var metric = await _dataContext.Metrics.FindAsync(request.MetricId);
            if (metric == null)
            {
                return null;
            }
            
            metric.AddDataPoint(request.Value, request.CreatedAt ?? DateTime.UtcNow);
            _dataContext.Metrics.Update(metric);
            
            await _dataContext.SaveChangesAsync(cancellationToken);

            return DataPointResource.FromDataPoint(metric.DataPoints.First());
        }
    }
}