using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MediatR;
using OpenNetworkStatus.Attributes;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Commands
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class AddComponentGroupCommand : IRequest<ComponentGroupResource>
    {
        [Required]
        public string Title { get; set; }
        
        [RequiredEnum]
        public ComponentGroupOptions ExpandOption { get; set; }

        public int Position { get; set; }

        public bool Display { get; set; }
    }
}