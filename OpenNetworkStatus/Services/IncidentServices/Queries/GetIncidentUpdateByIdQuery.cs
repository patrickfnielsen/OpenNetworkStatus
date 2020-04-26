using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Queries
{
    public class GetIncidentUpdateByIdQuery : IRequest<IncidentUpdateResource>
    {
        [Required]
        public int UpdateId { get; set; }
    }
}
