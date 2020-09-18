using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.MetricProviders.Repository;
using OpenNetworkStatus.Services.MetricServices.Commands;

namespace OpenNetworkStatus
{
    public class MetricProviderWorker : IHostedService, IDisposable
    {
        private readonly ILogger<MetricProviderWorker> _logger;
        private readonly IMetricProviderRepository _providerRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public MetricProviderWorker(ILogger<MetricProviderWorker> logger, IServiceScopeFactory scopeFactory,
            IMetricProviderRepository providerRepository)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _providerRepository = providerRepository;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service 'MetricProviderWorker' started at: {time}", DateTimeOffset.Now);

            _timer = new Timer(async (state) => await ExecuteAsync(cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service 'MetricProviderWorker' stopped at: {time}", DateTimeOffset.Now);

            return Task.CompletedTask;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await ExecuteMetricProviders(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unhandled exception occured");
            }
        }

        private async Task ExecuteMetricProviders(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StatusDataContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var metrics = await dbContext.Metrics.ToListAsync();

            foreach (var metric in metrics)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Service stop is requested");
                    return;
                }

                if (string.IsNullOrEmpty(metric.MetricProviderType) || metric.MetricProviderType == "custom")
                {
                    continue;
                }

                _logger.LogDebug("Running provider '{provider}' for metric '{metricId}'", metric.MetricProviderType, metric.Id);

                var provider = _providerRepository.GetMetricProvider(metric.MetricProviderType);
                if (provider == null)
                {
                    _logger.LogWarning("Unknown provider '{provider}', skipping", metric.MetricProviderType);
                    continue;
                }

                var dataPoint = await provider.PollDataPointAsync(metric);
                if(dataPoint == null)
                {
                    _logger.LogDebug("No datapoint returned from provider '{provider}' for metric '{metricId}'", metric.MetricProviderType, metric.Id);
                    continue;
                }

                var request = new AddDataPointCommand()
                {
                    MetricId = dataPoint.MetricId,
                    Value = dataPoint.Value,
                    CreatedAt = dataPoint.CreatedAt
                };

                await mediator.Send(request, stoppingToken);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}