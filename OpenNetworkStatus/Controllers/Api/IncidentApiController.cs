using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Exceptions;
using OpenNetworkStatus.Models;
using OpenNetworkStatus.Services.IncidentServices;
using OpenNetworkStatus.Services.IncidentServices.Resources;
using OpenNetworkStatus.Services.PageServices;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/incidents")]
    public class IncidentApiController : Controller
    {
        private readonly IncidentService _incidentService;
        
        public IncidentApiController(IncidentService incidentService)
        {
            _incidentService = incidentService;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<GetIncidentResource>> CreateAsync(AddIncidentResource incident)
        {
            var incidentResource = await _incidentService.CreateAsync(incident);

            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetIncident), 
                new { incidentId = incidentResource.Id, version = version},
                incidentResource);
        }
        
        [HttpDelete("{incidentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteIncident([FromRoute]int incidentId)
        {
            var incident = await _incidentService.GetIncidentAsync(incidentId);
            if (incident == null)
            {
                return NotFound();
            }
            
            await _incidentService.DeleteIncidentAsync(incidentId);
            return NoContent();
        }
        
        [HttpPut("{incidentId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateIncident([FromRoute]int incidentId, AddIncidentResource incident)
        {
            try
            {
                await _incidentService.UpdateIncidentAsync(incidentId, incident);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{incidentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetIncidentResource>> GetIncident([FromRoute]int incidentId)
        {
            var incident =  await _incidentService.GetIncidentAsync(incidentId);
            if (incident == null)
            {
                return NotFound();
            }

            return incident;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<GetIncidentResource>>> GetIncidents([FromQuery]PaginationQuery paginationQuery)
        {
            var result = await _incidentService.GetIncidentsAsync(paginationQuery.Page, paginationQuery.PerPage);
            return PageService.CreatePaginatedResponse(paginationQuery, result.ToList());
        }
        
        [HttpPost("{incidentId}/updates")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddIncidentUpdate([FromRoute]int incidentId, AddIncidentUpdateResource incidentUpdate)
        {
            try
            {
                var update = await _incidentService.AddIncidentUpdateAsync(incidentId, incidentUpdate);
                            
                var version = Request.HttpContext.GetRequestedApiVersion().ToString();
                return CreatedAtAction(
                    nameof(GetIncidentUpdate), 
                    new { incidentId = incidentId, updateId = update.Id, version = version },
                    update);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
        
        [HttpGet("{incidentId}/updates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResponse<GetIncidentUpdateResource>>> GetAllIncidentUpdates([FromRoute]int incidentId, [FromQuery]PaginationQuery paginationQuery)
        {
            var updates = await _incidentService.GetIncidentUpdatesAsync(incidentId, paginationQuery.Page, paginationQuery.PerPage);            
            return PageService.CreatePaginatedResponse(paginationQuery, updates);
        }

        [HttpGet("{incidentId}/updates/{updateId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetIncidentUpdateResource>> GetIncidentUpdate([FromRoute]int incidentId, [FromRoute]int updateId)
        {
            var update = await _incidentService.GetIncidentUpdateAsync(incidentId, updateId);
            if (update == null)
            {
                return NotFound();
            }
            
            return update;
        }
        
        [HttpPut("{incidentId}/updates/{updateId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetIncidentUpdateResource>> UpdateIncidentUpdate([FromRoute]int incidentId, [FromRoute]int updateId, [FromBody]AddIncidentUpdateResource update)
        {            
            try
            {
                await _incidentService.UpdateIncidentUpdateAsync(incidentId, updateId, update);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}