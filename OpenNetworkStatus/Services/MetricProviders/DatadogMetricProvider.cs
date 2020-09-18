using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.MetricProviders
{

    public class DataDogOptions
    {
        public string ApiKey { get; set; }
        public string AppKey { get; set; }
        public string DataDogEndpoint { get; set; } = "api.datadoghq.eu";
    }

    public class DataDogMetricResponse
    {
        public string Status { get; set; }
        public List<DataDogMetricSeries> Series { get; set; }
    }

    public class DataDogMetricSeries
    {
        public List<List<double>> PointList { get; set; }
    }

    public class DatadogMetricProvider : IMetricProvider
    {
        public string Name => "datadog";

        private readonly ILogger<DatadogMetricProvider> _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true };

        public DatadogMetricProvider(ILogger<DatadogMetricProvider> logger, IOptions<DataDogOptions> options)
        {
            _logger = logger;

            var endpoint = new UriBuilder("https", options.Value.DataDogEndpoint);

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = endpoint.Uri;
            _httpClient.DefaultRequestHeaders.Add("DD-API-KEY", options.Value.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("DD-APPLICATION-KEY", options.Value.AppKey);
        }

        public async ValueTask<DataPoint> PollDataPointAsync(Metric metric)
        {
            var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            using (var response = await _httpClient.GetAsync($"/api/v1/query?from={unixTime - 60}&to={unixTime}&query={metric.ExternalMetricIdentifier}"))
            {
                response.EnsureSuccessStatusCode();  // throws if not 200-299
                var data = await ParseToTypedResponseAsync(response);

                return ParseToDataPoint(metric.Id, data);
            }
        }

        private async ValueTask<DataDogMetricResponse> ParseToTypedResponseAsync(HttpResponseMessage response)
        {
            if (response.Content is object && response.Content.Headers.ContentType.MediaType == "application/json")
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                try
                {
                    return await JsonSerializer.DeserializeAsync<DataDogMetricResponse>(contentStream, _jsonSerializerOptions);
                }
                catch (JsonException e)
                {
                    _logger.LogError(e, "Invalid JSON");
                    return null;
                }
            }
            else
            {
                _logger.LogError("HTTP Response was invalid and cannot be deserialised");
                return null;
            }
        }

        private DataPoint ParseToDataPoint(int metricId, DataDogMetricResponse response)
        {
            if (response?.Status != "ok")
            {
                _logger.LogError("Invalid status returned from api. Expected 'ok' got '{status}'", response?.Status);
                return null;
            }

            if (response?.Series?.Count() == 0 || response.Series[0].PointList?.Count() == 0)
            {
                _logger.LogWarning("No timeseries data returned. Please check the query in ExternalMetricIdentifier");
                return null;
            }

            var point = response.Series.Last().PointList.Last();
            var unixTimeMilliseconds = Convert.ToInt64(point[0]);
            return new DataPoint
            {
                MetricId = metricId,
                Value = point[1],
                CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds).DateTime
            };
        }
    }
}
