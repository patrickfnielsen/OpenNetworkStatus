using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Queries
{
    public class GetIncidentByIdQuery : IRequest<IncidentResource>
    {
        [Required]
        public int IncidentId { get; set; }
    }
}
