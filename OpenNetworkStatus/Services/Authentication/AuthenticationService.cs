using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;

namespace OpenNetworkStatus.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly StatusDataContext _dataContext;
        private readonly PasswordHasher _passwordHasher;

        public AuthenticationService(StatusDataContext dataContext, PasswordHasher passwordHasher)
        {
            _dataContext = dataContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) //In theory this leaves us open to timing attacks.. We should consider refactoring this and user == null
            {
                return false;
            }

            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Username == username);

            // check if username exists
            if (user == null)
            {
                return false;
            }

            // check if password is correct
            if (_passwordHasher.VerifyHashedPassword(user.PasswordHash, password) == PasswordVerificationResult.Success)
            {
                return true;
            }

            return false;
        }
    }
}
