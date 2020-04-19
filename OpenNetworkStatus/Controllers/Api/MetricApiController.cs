using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.MetricServices.Commands;
using OpenNetworkStatus.Services.MetricServices.Queries;
using OpenNetworkStatus.Services.MetricServices.Resources;
using OpenNetworkStatus.Services.PageServices;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/metrics")]
    public class MetricApiController : Controller
    {
        private readonly ILogger<MetricApiController> _logger;
        private readonly StatusDataContext _dataContext;
        private readonly IMediator _mediator;
        
        public MetricApiController(ILogger<MetricApiController> logger, StatusDataContext dataContext, IMediator mediator)
        {
            _logger = logger;
            _dataContext = dataContext;
            _mediator = mediator;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<MetricResource>> CreateAsync(AddMetricCommand metricCommand)
        {
            var metricResource = await _mediator.Send(metricCommand);

            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetMetric), 
                new { metricId = metricResource.Id, version = version },
                metricResource);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMetric([FromRoute]DeleteMetricCommand metricCommand)
        {
            var metricResource = await _mediator.Send(metricCommand);
            if (metricResource == false)
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
        public async Task<ActionResult<MetricResource>> UpdateMetric([FromRoute]int id, UpdateMetricCommand metricCommand)
        {
            if (id != metricCommand.Id)
            {
                return BadRequest();
            }
            
            var metricResource = await _mediator.Send(metricCommand);
            if (metricResource == null)
            {
                return NotFound();
            }

            return metricResource;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MetricResource>> GetMetric([FromRoute]GetMetricByIdQuery metricQuery)
        {
            var metricResource = await _mediator.Send(metricQuery);
            if (metricResource == null)
            {
                return NotFound();
            }

            return metricResource;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<MetricResource>>> GetMetrics([FromQuery]GetAllMetricsQuery metricsQuery)
        {
            var metricResources = await _mediator.Send(metricsQuery);
            if (metricResources == null)
            {
                return NotFound();
            }
            
            return PageService.CreatePaginatedResponse(metricsQuery.Page, metricsQuery.Limit, metricResources);
        }
        
        [HttpGet("{metricId}/datapoints")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DataPointResource>>> GetDataPoints([FromRoute]GetDataPointsForLastDayQuery dataPointQuery)
        {
            var datapointResources = await _mediator.Send(dataPointQuery);
            if (datapointResources == null)
            {
                return NotFound();
            }
            
            return datapointResources;
        }

        [HttpPost("{id}/datapoints")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddDataPoints([FromRoute]int id, [FromBody]AddDataPointCommand dataPointCommand)
        {
            dataPointCommand.MetricId = id;

            var dataPointResource = await _mediator.Send(dataPointCommand);
        
            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetDataPoints), 
                new{ metricId = id, version = version },
                dataPointResource);
        }        
    }
}