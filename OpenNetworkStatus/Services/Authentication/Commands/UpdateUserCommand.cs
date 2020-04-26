using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.Authentication.Commands
{
    public class UpdateUserCommand : IRequest<UserResource>
    {
        internal int Id { get; set; }

        [Required]
        [MinLength(4)]
        public string Username { get; set; }

        [Required]
        [MinLength(7)]
        public string Password { get; set; }
    }
}
