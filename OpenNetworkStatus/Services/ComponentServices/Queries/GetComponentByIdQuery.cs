using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.ComponentServices.Resources;

namespace OpenNetworkStatus.Services.ComponentServices.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetComponentByIdQuery : IRequest<ComponentResource>
    {
        [Required]
        public int Id { get; set; }
    }
}