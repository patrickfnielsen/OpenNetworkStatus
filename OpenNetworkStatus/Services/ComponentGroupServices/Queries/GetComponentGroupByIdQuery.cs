using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetComponentGroupByIdQuery : IRequest<ComponentGroupResource>
    {
        [Required]
        public int Id { get; set; }
    }
}