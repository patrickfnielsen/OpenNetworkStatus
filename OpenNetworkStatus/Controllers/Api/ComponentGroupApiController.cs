using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.QueryObjects;

namespace OpenNetworkStatus.Controllers.Api
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/component-groups")]
    public class ComponentGroupGroupApiController : Controller
    {
        private readonly ILogger<ComponentGroupGroupApiController> _logger;
        private readonly StatusDataContext _dataContext;
        
        public ComponentGroupGroupApiController(ILogger<ComponentGroupGroupApiController> logger, StatusDataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ComponentGroup>> CreateAsync(ComponentGroup componentGroup)
        {
            _logger.LogInformation("Create ComponentGroup: {@componentGroup}", componentGroup);

            _dataContext.ComponentGroups.Add(componentGroup);
            await _dataContext.SaveChangesAsync();

            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetComponentGroup), 
                new { componentGroupId = componentGroup.Id, version = version },
                componentGroup);
        }
        
        [HttpDelete("{componentGroupId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComponentGroup([FromRoute]int componentGroupId)
        {
            var componentGroup = await _dataContext.ComponentGroups.FindAsync(componentGroupId);

            if (componentGroup == null)
            {
                _logger.LogDebug("Can't find componentGroup with id: {id}", componentGroupId);

                return NotFound();
            }

            _logger.LogInformation("Delete ComponentGroup: {@componentGroup}", componentGroup);

            _dataContext.ComponentGroups.Remove(componentGroup);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPut("{componentGroupId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComponentGroup([FromRoute]int componentGroupId, ComponentGroup componentGroup)
        {
            if (componentGroupId != componentGroup.Id)
            {
                return BadRequest();
            }
                        
            _dataContext.Entry(componentGroup).State = EntityState.Modified;
            
            _logger.LogInformation("Update ComponentGroup: {@componentGroup}", componentGroup);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ComponentGroupExists(componentGroupId))
            {
                _logger.LogDebug("Can't find componentGroup with id: {id}", componentGroupId);

                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{componentGroupId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComponentGroup>> GetComponentGroup([FromRoute]int componentGroupId)
        {
            _logger.LogInformation("Get ComponentGroup with id: {id}", componentGroupId);

            var result = await _dataContext.ComponentGroups.FindAsync(componentGroupId);

            if (result == null)
            {
                _logger.LogDebug("Can't find componentGroup with id: {id}", componentGroupId);
                return NotFound();
            }

            return result;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComponentGroup>>> GetComponentGroups([FromQuery]int page = 1, [FromQuery]int limit = 50)
        {
            var result = await _dataContext.ComponentGroups
                .Page(page, limit)
                .ComponentGroupOrder()
                .ToListAsync();

            return result;
        }

        private bool ComponentGroupExists(long id) => _dataContext.ComponentGroups.Any(e => e.Id == id);
    }
}