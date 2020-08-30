using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Services.Authentication;
using OpenNetworkStatus.Services.Authentication.Commands;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [Route("api/v{version:apiVersion}/authentication")]
    public class AuthenticationApiController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenManager _tokenManager;

        public AuthenticationApiController(IAuthenticationService authenticationService, ITokenManager tokenManager)
        {
            _authenticationService = authenticationService;
            _tokenManager = tokenManager;
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> LoginAsync([FromBody]LoginCommand loginResource)
        {
            var authResult = await _authenticationService.AuthenticateAsync(loginResource.Username, loginResource.Password);
            if (authResult == true)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginResource.Username)
                };

                var token = _tokenManager.GenerateToken(claims);
                return Ok(new TokenGrantedResource(token.Token, token.Expires));
            }

            return Forbid();
        }
    }
}