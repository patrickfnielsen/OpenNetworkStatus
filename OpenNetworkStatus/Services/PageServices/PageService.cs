using System.Collections.Generic;
using System.Linq;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Services.PageServices
{
    public static class PageService
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(int page, int perPage, List<T> response)
        {
            var nextPage = page >= 1 ? page + 1 : (int?)null;
            var previousPage = page- 1 >= 1 ? page - 1 : (int?)null;

            return new PagedResponse<T>
            {
                Items = response,
                PerPage = perPage,
                Page = page,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage
            };
        }
    }
}