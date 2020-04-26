using System.Threading.Tasks;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(string username, string password);
    }
}
