using System.ComponentModel.DataAnnotations;

namespace OpenNetworkStatus.Data.Entities
{
    public class Component : AuditEntityBase
    {
        public int Id { get; set; }
        
        public int? ComponentGroupId { get; set; }
        
        public int Order { get; set; }
        
        public ComponentStatus Status { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
    }
}