using System.Threading.Tasks;

namespace OpenNetworkStatus.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(string username, string password);
    }
}
