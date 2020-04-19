using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Attributes;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Commands
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class UpdateComponentGroupCommand : IRequest<ComponentGroupResource>
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
                
        public int Position { get; set; }
        
        public ComponentGroupOptions ExpandOption { get; set; }
    }
}