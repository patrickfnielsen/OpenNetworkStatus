using System.Collections.Generic;

namespace OpenNetworkStatus.Data.Entities
{
    public class ComponentGroup : AuditEntityBase
    {
        public int Id { get; set; }
        
        public int Order { get; set; }
        
        public string Name { get; set; }
                
        public ComponentGroupExpand ExpandOption { get; set; }
        
        public List<Component> Components { get; set; }
    }
}