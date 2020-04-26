using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace OpenNetworkStatus.Services.Authentication
{
    public interface ITokenManager
    {
        AuthToken GenerateToken(List<Claim> claims);
    }

    public struct AuthToken
    {
        public string Token { get; private set; }

        public DateTime Expires { get; private set; }

        public AuthToken(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }
    }
}