using System.ComponentModel.DataAnnotations;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class UpdateIncidentResource
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
    }
}