using System;
using System.Collections.Generic;
using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Data.Entities
{
    public class Incident : AuditEntityBase
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public List<IncidentUpdate> Updates { get; set; }
        
        public DateTime? ResolvedOn { get; set; }

        public void AddUpdate(IncidentStatus status, string message, StatusDataContext context = null) 
        {
            UpdatedOn = DateTime.UtcNow;
            
            if (Updates != null)    
            {
                Updates.Add(new IncidentUpdate
                {
                    IncidentId = Id,
                    Status = status,
                    Message = message,
                    CreatedOn = DateTime.UtcNow
                });   
            }
            else if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "You must provide a context if the Updates collection isn't valid.");
            }
            else if (context.Entry(this).IsKeySet)  
            {
                context.Add(new IncidentUpdate
                {
                    IncidentId = Id,
                    Status = status,
                    Message = message,
                    CreatedOn = DateTime.UtcNow
                });
            }
            else                                    
            {                                        
                throw new InvalidOperationException("Could not add a new Incident");  
            }
        }
    }
}