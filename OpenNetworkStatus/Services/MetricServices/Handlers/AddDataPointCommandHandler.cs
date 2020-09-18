using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Extensions;
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

            //We want to always round the time to the nearest 30 seconds.
            //If a datapoint already exists at that interval, replace the value with the new value
            //Else insert a new datapoint
            var createdAt = (request.CreatedAt ?? DateTime.UtcNow).RoundToNearest(TimeSpan.FromSeconds(30)); 
            var existingDataPoint = await _dataContext.DataPoints.FirstOrDefaultAsync(x => x.CreatedAt == createdAt);
            if (existingDataPoint != null)
            {
                existingDataPoint.Value = request.Value;

                _dataContext.Attach(existingDataPoint);
                _dataContext.Entry(existingDataPoint).Property(x => x.Value).IsModified = true;
            }
            else
            {
                metric.AddDataPoint(request.Value, createdAt, _dataContext);
                _dataContext.Metrics.Update(metric);
            }
            
            await _dataContext.SaveChangesAsync(cancellationToken);

            return DataPointResource.FromDataPoint(metric.DataPoints.First());
        }
    }
}