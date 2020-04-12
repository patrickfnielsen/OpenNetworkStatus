using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenNetworkStatus.Services.Exceptions;
using OpenNetworkStatus.Services.IncidentServices;
using OpenNetworkStatus.Services.IncidentServices.Resources;

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateIncident([FromRoute]int incidentId, UpdateIncidentResource incident)
        {
            if (incidentId != incident.Id)
            {
                return BadRequest();
            }
            
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
        public async Task<ActionResult<IEnumerable<GetIncidentResource>>> GetIncidents([FromQuery]int page = 1, [FromQuery]int limit = 50)
        {
            var result = await _incidentService.GetIncidentsAsync(page, limit);
            return result.ToList();
        }

        [HttpGet("{incidentId}/updates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<GetUpdateResource>>> GetIncidentUpdates([FromRoute]int incidentId)
        {
            var updates = await _incidentService.GetIncidentUpdatesAsync(incidentId);
            return updates.ToList();
        }
        
        [HttpGet("{incidentId}/updates/{updateId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetUpdateResource>> GetIncidentUpdate([FromRoute]int incidentId, [FromRoute]int updateId)
        {
            var update = await _incidentService.GetIncidentUpdateAsync(updateId);
            return update;
        }
        
        [HttpPost("{incidentId}/updates")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddIncidentUpdate([FromRoute]int incidentId, AddUpdateResource incidentUpdate)
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
    }
}