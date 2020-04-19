using System.Collections.Generic;

namespace OpenNetworkStatus.Services.PageServices.Resources
{
    public class PagedResponse<T>
    {
        public PagedResponse(){}

        public PagedResponse(IEnumerable<T> items)
        {
            Items = items;
        }

        public IEnumerable<T> Items { get; set; }

        public int Page { get; set; }

        public int PerPage { get; set; }

        public int? NextPage { get; set; }

        public int? PreviousPage { get; set; }
    }
}