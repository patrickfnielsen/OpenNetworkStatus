using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenNetworkStatus.Models.Options;

namespace OpenNetworkStatus.Services.Authentication
{
    public class JwtTokenManager : ITokenManager
    {
        private readonly JwtOptions _options;
        public JwtTokenManager(IOptions<SiteOptions> options)
        {
            _options = options.Value.Jwt;
        }
        
        public AuthToken GenerateToken(List<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_options.TokenLifetimeMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.ValidIssuer,
                audience: _options.ValidAudience,
                expires: expires,
                claims: claims,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return new AuthToken(stringToken, expires);
        }
    }
}