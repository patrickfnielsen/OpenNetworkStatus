using System;
using System.Linq;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Data.QueryObjects
{
    public static class DataPointsLast
    {
        public static IQueryable<DataPoint> GetDataPointsForLast(this IQueryable<DataPoint> queryable, int day, int metricId)
        {
            return queryable
                .Where(x => x.MetricId == metricId && x.CreatedAt >= DateTime.UtcNow.AddDays(-day))
                .OrderBy(x => x.CreatedAt);

        }
    }
}