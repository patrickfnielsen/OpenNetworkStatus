using System.ComponentModel.DataAnnotations;
using MediatR;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Commands
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class DeleteComponentGroupCommand : IRequest<bool>
    {
        [Required]
        public int Id { get; set; }
    }
}