using System;
using System.ComponentModel.DataAnnotations;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class GetUpdateResource
    {
        public int Id { get; set; }
                
        public IncidentStatus Status { get; set; }
        
        public string StatusText { get; set; }
        
        public string Message { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public DateTime UpdatedOn { get; set; }
    }
}