using System;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.MetricServices.Resources
{
    public class MetricResource
    {
        public int Id { get; set; }
        
        public int Position { get; set; }

        public int DecimalPlaces { get; set; }

        public string Title { get; set; }
        
        public string Suffix { get; set; }
        
        public string Description { get; set; }

        public bool Display { get; set; }

        public string MetricProviderType { get; set; }

        public string ExternalMetricIdentifier { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public static MetricResource FromMetric(Metric metric)
        {
            return new MetricResource
            {
                Id = metric.Id,
                Position = metric.Position,
                DecimalPlaces = metric.DecimalPlaces,
                Title = metric.Title,
                Suffix = metric.Suffix,
                Description = metric.Description,
                Display = metric.Display,
                MetricProviderType = metric.MetricProviderType,
                ExternalMetricIdentifier = metric.ExternalMetricIdentifier,
                CreatedAt = metric.CreatedAt,
                UpdatedAt = metric.UpdatedAt
            };
        }
    }
}