using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetAllIncidentsQuery : IRequest<List<IncidentResource>>
    {
        [Required]
        public int Page { get; set; }

        [Required]
        public int Limit { get; set; }

        public GetAllIncidentsQuery()
        {
            Page = 1;
            Limit = 50;
        }
    }
}
