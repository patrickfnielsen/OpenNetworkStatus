using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Attributes;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Commands
{
    public class UpdateIncidentUpdateCommand : IRequest<IncidentUpdateResource>
    {
        internal int Id { get; set; }

        [RequiredEnum]
        public IncidentStatus Status { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
