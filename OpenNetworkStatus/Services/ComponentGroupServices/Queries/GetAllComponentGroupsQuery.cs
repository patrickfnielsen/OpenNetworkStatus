using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetAllComponentGroupsQuery : IRequest<List<ComponentGroupResource>>
    {
        [Required]
        public int Page { get; set; }
        
        [Required]
        public int Limit { get; set; }

        public GetAllComponentGroupsQuery()
        {
            Page = 1;
            Limit = 50;
        }
    }
}