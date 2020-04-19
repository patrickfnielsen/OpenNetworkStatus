using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
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
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/component-groups")]
    public class ComponentGroupGroupApiController : Controller
    {
        private readonly IMediator _mediator;

        public ComponentGroupGroupApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ComponentGroup>> CreateAsync(AddComponentGroupCommand groupCommand)
        {
            var groupResource = await _mediator.Send(groupCommand);
            
            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetComponentGroup), 
                new { id = groupResource.Id, version = version },
                groupResource);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComponentGroup([FromRoute]DeleteComponentGroupCommand groupCommand)
        {
            var groupResource = await _mediator.Send(groupCommand);
            if (groupResource == false)
            {
                return NotFound();
            }

            return NoContent();
        }
        
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComponentGroupResource>> UpdateComponentGroup([FromRoute]int id, UpdateComponentGroupCommand groupCommand)
        {
            if (id != groupCommand.Id)
            {
                return BadRequest();
            }
            
            var groupResource = await _mediator.Send(groupCommand);
            if (groupResource == null)
            {
                return NotFound();
            }

            return groupResource;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComponentGroupResource>> GetComponentGroup([FromRoute]GetComponentGroupByIdQuery groupQuery)
        {
            var groupResource = await _mediator.Send(groupQuery);
            if (groupResource == null)
            {
                return NotFound();
            }

            return groupResource;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ComponentGroupResource>>> GetComponentGroups([FromQuery]GetAllComponentGroupsQuery groupQuery)
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