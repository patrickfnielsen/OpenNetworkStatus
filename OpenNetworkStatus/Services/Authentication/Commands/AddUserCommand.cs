using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.Authentication.Commands
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class AddUserCommand : IRequest<AddUserResource>
    {
        [Required]
        [MinLength(4)]
        public string Username { get; set; }

        [Required]
        [MinLength(7)]
        public string Password { get; set; }
    }
}
