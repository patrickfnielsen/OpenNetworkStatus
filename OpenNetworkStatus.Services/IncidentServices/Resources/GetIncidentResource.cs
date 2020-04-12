using System;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class GetIncidentResource
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public DateTime UpdatedOn { get; set; }
    }
}