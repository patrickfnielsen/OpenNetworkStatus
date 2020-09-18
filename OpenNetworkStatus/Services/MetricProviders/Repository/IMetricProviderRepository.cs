namespace OpenNetworkStatus.Services.MetricProviders.Repository
{
    public interface IMetricProviderRepository
    {
        public IMetricProvider GetMetricProvider(string type);
    }
}
