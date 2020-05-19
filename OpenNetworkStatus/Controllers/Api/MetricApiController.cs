using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Services.MetricServices.Commands;
using OpenNetworkStatus.Services.MetricServices.Queries;
using OpenNetworkStatus.Services.MetricServices.Resources;
using OpenNetworkStatus.Services.PageServices;
using OpenNetworkStatus.Services.PageServices.Resources;

namespace OpenNetworkStatus.Controllers.Api
{
    [Route("api/v{version:apiVersion}/metrics")]
    public class MetricApiController : BaseApiController
    {
        private readonly ILogger<MetricApiController> _logger;
        private readonly IMediator _mediator;
        
        public MetricApiController(ILogger<MetricApiController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<MetricResource>> CreateMetricAsync(AddMetricCommand metricCommand)
        {
            var metricResource = await _mediator.Send(metricCommand);

            return CreatedAtAction(
                nameof(GetMetricAsync), 
                new { metricId = metricResource.Id, version = RequestedApiVersion },
                metricResource);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMetricAsync([FromRoute]DeleteMetricCommand metricCommand)
        {
            var isDeleted = await _mediator.Send(metricCommand);
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
        public async Task<ActionResult<MetricResource>> UpdateMetricAsync([FromRoute]int id, UpdateMetricCommand metricCommand)
        {
            metricCommand.Id = id;

            var metricResource = await _mediator.Send(metricCommand);
            if (metricResource == null)
            {
                return NotFound();
            }

            return metricResource;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MetricResource>> GetMetricAsync([FromRoute]GetMetricByIdQuery metricQuery)
        {
            var metricResource = await _mediator.Send(metricQuery);
            if (metricResource == null)
            {
                return NotFound();
            }

            return metricResource;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<MetricResource>>> GetMetricsAsync([FromQuery]GetAllMetricsQuery metricsQuery)
        {
            var metricResources = await _mediator.Send(metricsQuery);
            if (metricResources == null)
            {
                return NotFound();
            }
            
            return PageService.CreatePaginatedResponse(metricsQuery.Page, metricsQuery.Limit, metricResources);
        }
        
        [HttpGet("{metricId}/datapoints")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DataPointResource>>> GetDataPointsAsync([FromRoute]GetDataPointsForLastDayQuery dataPointQuery)
        {
            var datapointResources = await _mediator.Send(dataPointQuery);
            if (datapointResources == null)
            {
                return NotFound();
            }
            
            return datapointResources;
        }

        [HttpGet("{metricId}/datapoints/week")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DataPointResource>>> GetDataPointsForLastWeekAsync([FromRoute]GetDataPointsForLastSevenDaysQuery dataPointQuery)
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
        public async Task<IActionResult> AddDataPointsAsync([FromRoute]int id, [FromBody]AddDataPointCommand dataPointCommand)
        {
            dataPointCommand.MetricId = id;

            var dataPointResource = await _mediator.Send(dataPointCommand);
        
            return CreatedAtAction(
                nameof(GetDataPointsAsync), 
                new{ metricId = id, version = RequestedApiVersion },
                dataPointResource);
        }        
    }
}