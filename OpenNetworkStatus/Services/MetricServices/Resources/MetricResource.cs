using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.MetricServices.Resources
{
    public class MetricResource
    {
        public int Id { get; set; }
        
        public int Position { get; set; }

        public string Title { get; set; }
        
        public string Suffix { get; set; }
        
        public string Description { get; set; }
        
        
        public static MetricResource FromMetric(Metric metric)
        {
            return new MetricResource
            {
                Id = metric.Id,
                Position = metric.Position,
                Title = metric.Title,
                Suffix = metric.Suffix,
                Description = metric.Description
            };
        }
    }
}