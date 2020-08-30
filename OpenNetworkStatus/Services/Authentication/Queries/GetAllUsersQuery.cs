using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.Authentication.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetAllUsersQuery : IRequest<List<UserResource>>
    {
        [Required]
        public int Page { get; set; }

        [Required]
        public int Limit { get; set; }

        public GetAllUsersQuery()
        {
            Page = 1;
            Limit = 50;
        }
    }
}
