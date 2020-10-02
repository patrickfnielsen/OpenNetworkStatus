using System.Collections.Generic;
using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Data.Entities
{
    public class ComponentGroup : AuditEntityBase
    {
        public int Id { get; set; }
        
        public int Position { get; set; }
        
        public string Name { get; set; }
                
        public ComponentGroupOptions ExpandOption { get; set; }
        
        public List<Component> Components { get; set; }

        public bool Display { get; set; }

    }
}