using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Services.Authentication;
using OpenNetworkStatus.Services.Authentication.Commands;
using OpenNetworkStatus.Services.Authentication.Queries;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [Route("api/v{version:apiVersion}/authentication")]
    public class AuthenticationApiController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenManager _tokenManager;
        private readonly IMediator _mediator;

        public AuthenticationApiController(IAuthenticationService authenticationService, ITokenManager tokenManager, IMediator mediator)
        {
            _authenticationService = authenticationService;
            _tokenManager = tokenManager;
            _mediator = mediator;
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

        [HttpPost("user")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody]AddUserCommand userCommand)
        {
            var result = await _mediator.Send(userCommand);

            if(!result.IsSuccess) {
                ModelState.AddModelError("Username", result.Message);
                return ValidationProblem();
            }

            return CreatedAtAction(
                nameof(GetUserAsync),
                new { id = result.User.Id, version = RequestedApiVersion },
                result.User);
        }

        [HttpGet("user/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResource>> GetUserAsync([FromRoute]GetUserByIdQuery userQuery)
        {
            var user = await _mediator.Send(userQuery);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPut("user/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResource>> UpdateUserAsync([FromRoute]int id, UpdateUserCommand userCommand)
        {
            userCommand.Id = id;
            
            var userResource = await _mediator.Send(userCommand);
            if (userResource == null)
            {
                return NotFound();
            }

            return userResource;
        }

        [HttpDelete("user/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComponentAsync([FromRoute]DeleteUserCommand userCommand)
        {
            var isDeleted = await _mediator.Send(userCommand);
            if (isDeleted == false)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}