using System;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Services.ComponentServices.Resources
{
    public class ComponentResource
    {
        public int Id { get; set; }
        
        public int? GroupId { get; set; }
        
        public int Position { get; set; }
            
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public ComponentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        public static ComponentResource FromComponent(Component component)
        {
            return new ComponentResource
            {
                Id = component.Id,
                Title = component.Title,
                Description = component.Description,
                Status = component.Status,
                GroupId = component.ComponentGroupId,
                Position = component.Position,
                CreatedAt = component.CreatedAt,
                UpdatedAt = component.UpdatedAt
            };
        }
    }
}