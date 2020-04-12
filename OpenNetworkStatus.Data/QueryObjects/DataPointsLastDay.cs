using System;
using System.Linq;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Data.QueryObjects
{
    public static class DataPointsLastDay
    {
        public static IQueryable<DataPoint> GetDataPointsLastDay(this IQueryable<DataPoint> queryable, int metricId)
        {
            return queryable
                .Where(x => x.MetricId == metricId && x.CreatedOn >= DateTime.UtcNow.AddDays(-1))
                .OrderBy(x => x.CreatedOn);

        }
    }
}