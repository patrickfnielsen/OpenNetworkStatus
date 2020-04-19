using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OpenNetworkStatus.Attributes;
using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class AddIncidentResource
    {
        public AddIncidentResource()
        {
            Impact = IncidentImpact.None;
        }
        
        [Required]
        public string Title { get; set; }
        
        [RequiredEnum]
        public IncidentImpact Impact { get; set; }
                
        public DateTime? ResolvedAt { get; set; }
    }
    
    public class GetIncidentResource
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        public IncidentImpact Impact { get; set; }

        public List<GetIncidentUpdateResource> Updates { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public DateTime? ResolvedAt { get; set; }
    }
}