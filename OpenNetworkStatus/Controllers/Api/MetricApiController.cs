using System;
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
    [Route("api/v{version:apiVersion}/metrics")]
    public class MetricApiController : Controller
    {
        private readonly ILogger<MetricApiController> _logger;
        private readonly StatusDataContext _dataContext;
        
        public MetricApiController(ILogger<MetricApiController> logger, StatusDataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Metric>> CreateAsync(Metric metric)
        {
            _logger.LogInformation("Create Metric: {@metric}", metric);

            _dataContext.Metrics.Add(metric);
            await _dataContext.SaveChangesAsync();

            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetMetric), 
                new { metricId = metric.Id, version = version },
                metric);
        }
        
        [HttpDelete("{metricId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMetric([FromRoute]int metricId)
        {
            var metric = await _dataContext.Metrics.FindAsync(metricId);

            if (metric == null)
            {
                _logger.LogDebug("Can't find metric with id: {id}", metricId);

                return NotFound();
            }

            _logger.LogInformation("Delete Metric: {@metric}", metric);

            _dataContext.Metrics.Remove(metric);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPut("{metricId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMetric([FromRoute]int metricId, Metric metric)
        {
            if (metricId != metric.Id)
            {
                return BadRequest();
            }
                        
            _dataContext.Entry(metric).State = EntityState.Modified;
            
            _logger.LogInformation("Update Metric: {@metric}", metric);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!MetricExists(metricId))
            {
                _logger.LogDebug("Can't find metric with id: {id}", metricId);

                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{metricId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Metric>> GetMetric([FromRoute]int metricId)
        {
            _logger.LogInformation("Get Metric with id: {id}", metricId);

            var result = await _dataContext.Metrics.FindAsync(metricId);

            if (result == null)
            {
                _logger.LogDebug("Can't find metric with id: {id}", metricId);
                return NotFound();
            }

            return result;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Metric>>> GetMetrics([FromQuery]int page = 1, [FromQuery]int limit = 50)
        {
            var result = await _dataContext.Metrics
                .Page(page, limit)
                .MetricOrder()
                .ToListAsync();

            return result;
        }
        
        [HttpGet("{metricId}/datapoints")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DataPoint>>> GetDataPoints(int metricId)
        {
            var datapoints = await _dataContext.DataPoints.GetDataPointsLastDay(metricId).ToListAsync();
            
            if (datapoints == null)
            {
                _logger.LogDebug("Can't find datapoints for metric with id: {id}", metricId);
                return NotFound();
            }
            
            return datapoints;
        }

        [HttpPost("{metricId}/datapoints")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddDataPoints(int metricId, List<DataPoint> dataPoints)
        {
            var metric = await _dataContext.Metrics.FindAsync(metricId);
            if (metric == null)
            {
                _logger.LogDebug("Can't find metric with id: {id}", metricId);
                return NotFound();
            }
            
            dataPoints.ForEach(x => metric.AddDataPoint(x.Value, x.CreatedOn, _dataContext));

            _dataContext.Metrics.Update(metric);
            
            _logger.LogInformation("Update Metric DataPoints: {@dataPoints}", dataPoints);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!MetricExists(metricId))
            {
                _logger.LogDebug("Can't find metric with id: {id}", metricId);

                return NotFound();
            }

            var version = Request.HttpContext.GetRequestedApiVersion().ToString();
            return CreatedAtAction(
                nameof(GetDataPoints), 
                new{ metricId = metric.Id, version = version },
                metric.DataPoints);
        }
        
        private bool MetricExists(long id) => _dataContext.Metrics.Any(e => e.Id == id);
    }
}