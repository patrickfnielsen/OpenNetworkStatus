using System;

namespace OpenNetworkStatus.Services.Authentication.Resources
{
    public class TokenGrantedResource
    {
        public string Token { get; }
        public DateTime Expires { get; set; }

        public TokenGrantedResource(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }
    }
}
