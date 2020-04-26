using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Queries
{
    public class GetIncidentUpdatesForIncidentQuery : IRequest<List<IncidentUpdateResource>>
    {
        [Required]
        public int IncidentId { get; set; }

        [Required]
        public int Page { get; set; }

        [Required]
        public int Limit { get; set; }

        public GetIncidentUpdatesForIncidentQuery()
        {
            Page = 1;
            Limit = 50;
        }
    }
}
