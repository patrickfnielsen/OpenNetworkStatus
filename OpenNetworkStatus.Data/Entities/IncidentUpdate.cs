using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Data.Entities
{
    public class IncidentUpdate : AuditEntityBase
    {
        public int Id { get; set; }
        
        public int IncidentId { get; set; }
        
        public IncidentStatus Status { get; set; }
        
        public string Message { get; set; }   
    }
}