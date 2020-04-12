using System;
using System.Linq;

namespace OpenNetworkStatus.Data.QueryObjects
{
    public static class GenericPage
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, int page, int limit)
        {
            if (limit > 100)
            {
                limit = 100;
            }
            
            var skipCount = Math.Max(0, page - 1) * limit;
            return queryable
                .Skip(skipCount)
                .Take(limit);
        }
    }
}