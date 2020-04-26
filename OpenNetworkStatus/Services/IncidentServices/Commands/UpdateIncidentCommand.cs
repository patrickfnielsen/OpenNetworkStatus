using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Attributes;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Commands
{
    public class UpdateIncidentCommand : IRequest<IncidentResource>
    {
        internal int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [RequiredEnum]
        public IncidentImpact Impact { get; set; }

        public DateTime? ResolvedAt { get; set; }
    }
}
