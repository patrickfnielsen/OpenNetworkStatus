
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Attributes;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentServices.Commands
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class UpdateComponentStatusCommand : IRequest<ComponentResource>
    {
        internal int Id { get; set; }
            
        [RequiredEnum]
        public ComponentStatus Status { get; set; }
    }
}