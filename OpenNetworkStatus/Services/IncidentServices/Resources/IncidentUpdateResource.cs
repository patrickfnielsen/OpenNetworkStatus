using System;
using System.ComponentModel.DataAnnotations;
using OpenNetworkStatus.Attributes;
using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class AddIncidentUpdateResource
    {        
        [RequiredEnum]
        public IncidentStatus Status { get; set; }

        [Required]
        public string Message { get; set; }
    }
    
    public class GetIncidentUpdateResource
    {
        public int Id { get; set; }
        
        public int IncidentId { get; set; }
                
        public IncidentStatus Status { get; set; }
        
        public string StatusText { get; set; }
        
        public string Message { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}