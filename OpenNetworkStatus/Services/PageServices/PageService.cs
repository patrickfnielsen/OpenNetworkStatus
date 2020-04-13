using System.Collections.Generic;
using System.Linq;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Services.PageServices
{
    public static class PageService
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(PaginationQuery pagination, List<T> response)
        {
            var nextPage = pagination.Page >= 1 ? pagination.Page + 1 : (int?)null;
            var previousPage = pagination.Page - 1 >= 1 ? pagination.Page - 1 : (int?)null;

            return new PagedResponse<T>
            {
                Items = response,
                PerPage = pagination.PerPage,
                Page = pagination.Page,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage
            };
        }
    }
}