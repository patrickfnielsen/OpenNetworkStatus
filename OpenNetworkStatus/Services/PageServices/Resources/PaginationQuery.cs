namespace OpenNetworkStatus.Services.PageServices.Resources
{
    public class PaginationQuery
    {
        public PaginationQuery()
        {
            Page = 1;
            PerPage = 100;
        }

        public PaginationQuery(int page, int perPage)
        {
            Page = page;
            PerPage = perPage;
        }

        public int Page { get; set; }

        public int PerPage { get; set; }
    }
}