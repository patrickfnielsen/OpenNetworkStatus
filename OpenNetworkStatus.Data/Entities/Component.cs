using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Data.Entities
{
    public class Component : AuditEntityBase
    {
        public int Id { get; set; }
        
        public int? ComponentGroupId { get; set; }
        
        public int Position { get; set; }
        
        public ComponentStatus Status { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public bool Display { get; set; }
    }
}