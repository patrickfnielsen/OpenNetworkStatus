using System.ComponentModel.DataAnnotations;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class AddIncidentResource
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Message { get; set; }
    }
}