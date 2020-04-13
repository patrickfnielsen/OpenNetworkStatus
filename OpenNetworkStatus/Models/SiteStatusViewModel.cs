using OpenNetworkStatus.Models.Enums;
using OpenNetworkStatus.Services;

namespace OpenNetworkStatus.Models
{
    public class SiteStatusViewModel
    {
        public SiteStatusViewModel(SiteStatus status)
        {
            Status = status;
            StatusText = EnumTextService.GetSiteStatus(status);
        }

        public SiteStatus Status { get; }
        public string StatusText { get; }
    }
}