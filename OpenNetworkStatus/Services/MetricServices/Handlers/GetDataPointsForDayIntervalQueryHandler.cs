using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Extensions;
using OpenNetworkStatus.Services.MetricServices.Queries;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class GetDataPointsForDayIntervalQueryHandler : IRequestHandler<GetDataPointsForDayIntervalQuery, List<DataPointResource>>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetDataPointsForDayIntervalQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<DataPointResource>> Handle(GetDataPointsForDayIntervalQuery request, CancellationToken cancellationToken)
        {
            var dataPoints = await _dataContext.DataPoints
                .GetDataPointsForLast(request.Days, request.MetricId).AsNoTracking().ToListAsync(cancellationToken);


            dataPoints = AddMissingDataPointIntervals(dataPoints, request.Days, request.MetricId, request.Interval);
            dataPoints = NormalizeDataPointIntervals(dataPoints, request.MetricId, request.Interval);

            return dataPoints.Select(DataPointResource.FromDataPoint).ToList();
        }

        public List<DataPoint> AddMissingDataPointIntervals(List<DataPoint> currentDataPoints, int day, int metricId, int interval)
        {
            //If we are missing data, for any reason - we want to fill in blanks
            //We do this my generating blank intervals for all possible combination and then union it with the primary list.
            //TODO: This seems really in efficient.. Please fix it
            var now = DateTime.UtcNow;
            var nullDataPoints = Enumerable
                .Range(0, (int)(TimeSpan.FromDays(day).TotalSeconds / interval))
                .Select(i => new DataPoint
                {
                    MetricId = metricId,
                    CreatedAt = now.AddDays(-day).AddSeconds(i * interval).RoundToNearest(TimeSpan.FromSeconds(interval)),
                    Value = -1
                }).ToList();

            return currentDataPoints.Union(nullDataPoints).OrderBy(x => x.CreatedAt).ToList();
        }

        public List<DataPoint> NormalizeDataPointIntervals(List<DataPoint> dataPoints, int metricId, int interval)
        {
            //Generate groups with our interval (ie. 5 minutes)
            //From each group generate a new datapoint and select the average value
            return dataPoints.GroupBy(x =>
            {
                return x.CreatedAt.RoundToNearest(TimeSpan.FromSeconds(interval));
            })
            .Select(g => new DataPoint { MetricId = metricId, CreatedAt = g.Key, Value = g.Max(s => s.Value) })
            .ToList();
        }
    }
}