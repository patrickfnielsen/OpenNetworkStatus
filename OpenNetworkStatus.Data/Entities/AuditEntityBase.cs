using System;

namespace OpenNetworkStatus.Data.Entities
{
    public class AuditEntityBase
    {
        public DateTime CreatedOn { get; set; }
        
        public DateTime UpdatedOn { get; set; }
    }
}