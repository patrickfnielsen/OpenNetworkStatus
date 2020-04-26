using System.ComponentModel.DataAnnotations;

namespace OpenNetworkStatus.Services.Authentication.Commands
{
    public class LoginCommand
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
