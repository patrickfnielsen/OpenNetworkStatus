using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Data.QueryObjects
{
    public static class IncidentPeriod
    {
        public static IQueryable<Incident> GetIncidentsLast(this IQueryable<Incident> queryable, int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            return queryable
                .Include(x => x.Updates)
                .Where(x => x.CreatedAt.Date >= startDate)
                .IncidentOrder();
                
        }
    }
}