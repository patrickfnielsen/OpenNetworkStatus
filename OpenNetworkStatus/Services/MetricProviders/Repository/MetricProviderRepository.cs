using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OpenNetworkStatus.Services.MetricProviders.Repository
{
    public class MetricProviderRepository : IMetricProviderRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, IMetricProvider> _metricProviders;
        public MetricProviderRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _metricProviders = GetMetricProviders();
        }

        public IMetricProvider GetMetricProvider(string type)
        {
            _metricProviders.TryGetValue(type, out var provider);

            return provider;
        }

        private Dictionary<string, IMetricProvider> GetMetricProviders()
        {
            var implementations = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                                  .Where(x => typeof(IMetricProvider).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

            var items = new Dictionary<string, IMetricProvider>();
            foreach(var item in implementations)
            {
                var provider = ActivatorUtilities.CreateInstance(_serviceProvider, item) as IMetricProvider;
                items.Add(provider.Name, provider);
            }

            return items;
        }
    }
}
