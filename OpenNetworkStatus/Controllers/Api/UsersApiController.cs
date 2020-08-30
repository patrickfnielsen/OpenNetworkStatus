using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Services.Authentication.Commands;
using OpenNetworkStatus.Services.Authentication.Queries;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [Route("api/v{version:apiVersion}/users")]
    public class UsersApiController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<UserResource>>> GeAllUserAsync([FromRoute] GetAllUsersQuery userQuery)
        {
            var user = await _mediator.Send(userQuery);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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