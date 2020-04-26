using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Services.ComponentServices.Commands;
using OpenNetworkStatus.Services.ComponentServices.Queries;
using OpenNetworkStatus.Services.ComponentServices.Resources;
using OpenNetworkStatus.Services.PageServices;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [Route("api/v{version:apiVersion}/components")]
    public class ComponentApiController : BaseApiController
    {
        private readonly IMediator _mediator;
        
        public ComponentApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ComponentResource>> CreateComponentAsync([FromBody]AddComponentCommand componentCommand)
        {
            var componentResource = await _mediator.Send(componentCommand);

            return CreatedAtAction(
                nameof(GetComponentAsync), 
                new { id = componentResource.Id, version = RequestedApiVersion },
                componentResource);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComponentAsync([FromRoute]DeleteComponentCommand componentCommand)
        {
            var isDeleted = await _mediator.Send(componentCommand);
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
        public async Task<ActionResult<ComponentResource>> UpdateComponentAsync([FromRoute]int id, UpdateComponentCommand componentCommand)
        {
            componentCommand.Id = id;
            
            var componentResource = await _mediator.Send(componentCommand);
            if (componentResource == null)
            {
                return NotFound();
            }

            return componentResource;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComponentResource>> GetComponentAsync([FromRoute]GetComponentByIdQuery componentQuery)
        {
            var componentResource = await _mediator.Send(componentQuery);
            if (componentResource == null)
            {
                return NotFound();
            }

            return componentResource;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ComponentResource>>> GetComponentsAsync([FromQuery]GetAllComponentsQuery componentQuery)
        {
            var componentResource = await _mediator.Send(componentQuery);
            if (componentResource == null)
            {
                return NotFound();
            }
            
            return PageService.CreatePaginatedResponse(componentQuery.Page, componentQuery.Limit, componentResource);
        }
    }
}