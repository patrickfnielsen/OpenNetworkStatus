using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.ComponentGroupServices.Commands;
using OpenNetworkStatus.Services.ComponentGroupServices.Queries;
using OpenNetworkStatus.Services.ComponentGroupServices.Resources;
using OpenNetworkStatus.Services.PageServices;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [Route("api/v{version:apiVersion}/component-groups")]
    public class ComponentGroupGroupApiController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ComponentGroupGroupApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ComponentGroup>> CreateComponentGroupAsync(AddComponentGroupCommand groupCommand)
        {
            var groupResource = await _mediator.Send(groupCommand);
            
            return CreatedAtAction(
                nameof(GetComponentGroupAsync), 
                new { id = groupResource.Id, version = RequestedApiVersion },
                groupResource);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComponentGroupAsync([FromRoute]DeleteComponentGroupCommand groupCommand)
        {
            var isDeleted = await _mediator.Send(groupCommand);
            if (isDeleted == false)
            {
                return NotFound();
            }

            return NoContent();
        }
        
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComponentGroupResource>> UpdateComponentGroupAsync([FromRoute]int id, UpdateComponentGroupCommand groupCommand)
        {
            groupCommand.Id = id;
            
            var groupResource = await _mediator.Send(groupCommand);
            if (groupResource == null)
            {
                return NotFound();
            }

            return groupResource;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComponentGroupResource>> GetComponentGroupAsync([FromRoute]GetComponentGroupByIdQuery groupQuery)
        {
            var groupResource = await _mediator.Send(groupQuery);
            if (groupResource == null)
            {
                return NotFound();
            }

            return groupResource;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ComponentGroupResource>>> GetComponentGroupsAsync([FromQuery]GetAllComponentGroupsQuery groupQuery)
        {
            var groupResources = await _mediator.Send(groupQuery);
            if (groupResources == null)
            {
                return NotFound();
            }
            
            return PageService.CreatePaginatedResponse(groupQuery.Page, groupQuery.Limit, groupResources);
        }
    }
}