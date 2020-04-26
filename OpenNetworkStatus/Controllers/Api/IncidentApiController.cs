using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Services.IncidentServices.Commands;
using OpenNetworkStatus.Services.IncidentServices.Queries;
using OpenNetworkStatus.Services.IncidentServices.Resources;
using OpenNetworkStatus.Services.PageServices;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [Route("api/v{version:apiVersion}/incidents")]
    public class IncidentApiController : BaseApiController
    {
        private readonly IMediator _mediator;
        
        public IncidentApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<IncidentResource>> CreateIncidentAsync(AddIncidentCommand incidentCommand)
        {
            var incidentResource = await _mediator.Send(incidentCommand);

            return CreatedAtAction(
                nameof(GetIncidentAsync), 
                new { id = incidentResource.Id, version = RequestedApiVersion },
                incidentResource);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIncidentAsync([FromRoute]DeleteIncidentCommand incidentCommand)
        {
            var isDeleted = await _mediator.Send(incidentCommand);
            if (isDeleted == false)
            {
                return NotFound();
            }

            return NoContent();
        }
        
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidentResource>> UpdateIncidentAsync([FromRoute]int id, [FromBody]UpdateIncidentCommand incidentCommand)
        {
            incidentCommand.Id = id;

            var incidentResource = await _mediator.Send(incidentCommand);
            if (incidentResource == null)
            {
                return NotFound();
            }

            return incidentResource;
        }

        [HttpGet("{incidentId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidentResource>> GetIncidentAsync([FromRoute]GetIncidentByIdQuery incidentQuery)
        {
            var incidentResource = await _mediator.Send(incidentQuery);
            if (incidentResource == null)
            {
                return NotFound();
            }

            return incidentResource;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<IncidentResource>>> GetIncidentsAsync([FromQuery]GetAllIncidentsQuery incidentQuery)
        {
            var incidentResources = await _mediator.Send(incidentQuery);
            if (incidentResources == null)
            {
                return NotFound();
            }

            return PageService.CreatePaginatedResponse(incidentQuery.Page, incidentQuery.Limit, incidentResources);
        }
        
        [HttpPost("{incidentId}/updates")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddIncidentUpdateAsync([FromRoute]int incidentId, AddIncidentUpdateCommand incidentUpdateCommand)
        {
            incidentUpdateCommand.IncidentId = incidentId;

            var incidentResource = await _mediator.Send(incidentUpdateCommand);
            if(incidentResource == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetIncidentUpdateAsync),
                new { incidentId = incidentResource.IncidentId, updateid = incidentResource.Id, version = RequestedApiVersion },
                incidentResource);
        }
        
        [HttpGet("{incidentId}/updates")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResponse<IncidentUpdateResource>>> GetAllIncidentUpdates([FromRoute]GetIncidentUpdatesForIncidentQuery incidentUpdateQuery)
        {
            var updateResources = await _mediator.Send(incidentUpdateQuery);         
            return PageService.CreatePaginatedResponse(incidentUpdateQuery.Page, incidentUpdateQuery.Limit, updateResources);
        }

        [HttpGet("{incidentId}/updates/{updateId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidentUpdateResource>> GetIncidentUpdateAsync([FromRoute]GetIncidentUpdateByIdQuery incidentUpdateQuery)
        {
            var updateResource = await _mediator.Send(incidentUpdateQuery);
            if (updateResource == null)
            {
                return NotFound();
            }
            
            return updateResource;
        }
        
        [HttpPut("{incidentId}/updates/{updateId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidentUpdateResource>> UpdateIncidentUpdateAsync([FromRoute]int incidentId, [FromRoute]int updateId, [FromBody]UpdateIncidentUpdateCommand incidentUpdateCommand)
        {
            incidentUpdateCommand.Id = updateId;
            incidentUpdateCommand.IncidentId = incidentId;

            var updateResource = await _mediator.Send(incidentUpdateCommand);
            if (updateResource == null)
            {
                return NotFound();
            }

            return updateResource;
        }
    }
}