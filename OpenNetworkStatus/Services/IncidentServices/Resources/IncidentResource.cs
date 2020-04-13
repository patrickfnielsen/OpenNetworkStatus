using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class AddIncidentResource
    {        
        [Required]
        public string Title { get; set; }
        
        public DateTime? ResolvedOn { get; set; }

    }
    
    public class GetIncidentResource
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        public List<GetIncidentUpdateResource> Updates { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public DateTime UpdatedOn { get; set; }
        
        public DateTime? ResolvedOn { get; set; }
    }
}