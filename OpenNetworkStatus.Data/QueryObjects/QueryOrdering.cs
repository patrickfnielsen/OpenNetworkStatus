using System.Linq;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Data.QueryObjects
{
    public static class QueryOrdering
    {
        public static IQueryable<Metric> MetricOrder(this IQueryable<Metric> queryable) 
        {
            return queryable
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Title);
        }
        
        public static IQueryable<Component> ComponentOrder(this IQueryable<Component> queryable) 
        {
            return queryable
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Title);
        }
        
        public static IQueryable<ComponentGroup> ComponentGroupOrder(this IQueryable<ComponentGroup> queryable) 
        {
            return queryable
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name);
        }
        
        public static IQueryable<Incident> IncidentOrder(this IQueryable<Incident> queryable) 
        {
            return queryable
                .OrderBy(x => x.CreatedOn)
                .ThenBy(x => x.Id);
        }
    }
}