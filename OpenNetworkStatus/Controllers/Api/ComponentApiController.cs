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
    [Route("api/v{version:apiVersion}/components")]
    public class ComponentApiController : Controller
    {
        private readonly ILogger<ComponentApiController> _logger;
        private readonly StatusDataContext _dataContext;
        
        public ComponentApiController(ILogger<ComponentApiController> logger, StatusDataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Component>> CreateAsync(Component component)
        {
            _logger.LogInformation("Create Component: {@component}", component);

            _dataContext.Components.Add(component);
            await _dataContext.SaveChangesAsync();

            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetComponent), 
                new { componentId = component.Id, version = version },
                component);
        }
        
        [HttpDelete("{componentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComponent([FromRoute]int componentId)
        {
            var component = await _dataContext.Components.FindAsync(componentId);

            if (component == null)
            {
                _logger.LogDebug("Can't find component with id: {id}", componentId);

                return NotFound();
            }

            _logger.LogInformation("Delete Component: {@component}", component);

            _dataContext.Components.Remove(component);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPut("{componentId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComponent([FromRoute]int componentId, Component component)
        {
            if (componentId != component.Id)
            {
                return BadRequest();
            }
                        
            _dataContext.Entry(component).State = EntityState.Modified;
            
            _logger.LogInformation("Update Component: {@component}", component);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ComponentExists(componentId))
            {
                _logger.LogDebug("Can't find component with id: {id}", componentId);

                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{componentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Component>> GetComponent([FromRoute]int componentId)
        {
            _logger.LogInformation("Get Component with id: {id}", componentId);

            var result = await _dataContext.Components.FindAsync(componentId);

            if (result == null)
            {
                _logger.LogDebug("Can't find component with id: {id}", componentId);
                return NotFound();
            }

            return result;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Component>>> GetComponents([FromQuery]int page = 1, [FromQuery]int limit = 50)
        {
            var result = await _dataContext.Components
                .Page(page, limit)
                .ComponentOrder()
                .ToListAsync();

            return result;
        }

        private bool ComponentExists(long id) => _dataContext.Components.Any(e => e.Id == id);
    }
}