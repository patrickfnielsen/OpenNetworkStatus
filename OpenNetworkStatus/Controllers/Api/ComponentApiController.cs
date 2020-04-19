using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Services.ComponentServices.Commands;
using OpenNetworkStatus.Services.ComponentServices.Queries;
using OpenNetworkStatus.Services.ComponentServices.Resources;
using OpenNetworkStatus.Services.PageServices;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/components")]
    public class ComponentApiController : Controller
    {
        private readonly IMediator _mediator;
        
        public ComponentApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ComponentResource>> CreateAsync([FromBody]AddComponentCommand componentCommand)
        {
            var componentResource = await _mediator.Send(componentCommand);

            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetComponent), 
                new { id = componentResource.Id, version = version },
                componentResource);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComponent([FromRoute]DeleteComponentCommand componentCommand)
        {
            var componentResource = await _mediator.Send(componentCommand);

            if (componentResource == false)
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
        public async Task<ActionResult<ComponentResource>> UpdateComponent([FromRoute]int id, UpdateComponentCommand componentCommand)
        {
            if (id != componentCommand.Id)
            {
                return BadRequest();
            }
            
            var componentResource = await _mediator.Send(componentCommand);
            if (componentResource == null)
            {
                return NotFound();
            }

            return componentResource;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComponentResource>> GetComponent([FromRoute]GetComponentByIdQuery componentQuery)
        {
            var componentResource = await _mediator.Send(componentQuery);
            if (componentResource == null)
            {
                return NotFound();
            }

            return componentResource;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ComponentResource>>> GetComponents([FromQuery]GetAllComponentsQuery componentQuery)
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