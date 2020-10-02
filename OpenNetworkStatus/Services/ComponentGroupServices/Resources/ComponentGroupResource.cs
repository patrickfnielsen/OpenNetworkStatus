using System;
using System.Collections.Generic;
using System.Linq;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Resources
{
    public class ComponentGroupResource
    {
        public int Id { get; set; }
            
        public string Title { get; set; }

        public bool Display { get; set; }

        public ComponentGroupOptions ExpandOption { get; set; }

        public int Position { get; set; }
            
        public List<ComponentResource> Components { get; set; }
            
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        public ComponentGroupResource()
        {
            Components = new List<ComponentResource>();
        }
        
        public static ComponentGroupResource FromComponentGroup(ComponentGroup group)
        {
            var components = group.Components != null ?
                group.Components.Select(ComponentResource.FromComponent).ToList() : 
                new List<ComponentResource>();

            return new ComponentGroupResource
            {
                Id = group.Id,
                Title = group.Name,
                Display = group.Display,
                ExpandOption = group.ExpandOption,
                Components = components,
                Position = group.Position,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };
        }
    }
}