using System.Threading.Tasks;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.MetricProviders
{
    public interface IMetricProvider
    {
        string Name { get; }
        ValueTask<DataPoint> PollDataPointAsync(Metric metric);
    }
}
