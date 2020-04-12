using System.ComponentModel.DataAnnotations;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.Attributes;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class AddUpdateResource
    {
        [RequiredEnum]
        public IncidentStatus Status { get; set; }

        [Required]
        public string Message { get; set; }
    }
}