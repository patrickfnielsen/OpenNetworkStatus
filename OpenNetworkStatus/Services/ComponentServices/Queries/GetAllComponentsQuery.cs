using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentServices.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetAllComponentsQuery : IRequest<List<ComponentResource>>
    {
        [Required]
        public int Page { get; set; }
        
        [Required]
        public int Limit { get; set; }

        public GetAllComponentsQuery()
        {
            Page = 1;
            Limit = 50;
        }
    }
}