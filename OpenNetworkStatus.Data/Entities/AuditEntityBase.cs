using System;

namespace OpenNetworkStatus.Data.Entities
{
    public class AuditEntityBase
    {
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}