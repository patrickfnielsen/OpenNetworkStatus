using System;
using System.Collections.Generic;

namespace OpenNetworkStatus.Data.Entities
{
    public class Metric : AuditEntityBase
    {
        public int Id { get; set; }
        
        public int Position { get; set; }

        public int DecimalPlaces { get; set; } = 2;

        public string Title { get; set; }
        
        public string Suffix { get; set; }
        
        public string Description { get; set; }

        public bool Display { get; set; }

        public string MetricProviderType { get; set; } = "custom";

        public string ExternalMetricIdentifier { get; set; }

        public List<DataPoint> DataPoints { get; set; }
        
        public void AddDataPoint(double value, DateTime createdAt, StatusDataContext context = null)
        {            
            if (DataPoints != null)
            {
                DataPoints.Add(new DataPoint
                {
                    Value = value,
                    MetricId = Id,
                    CreatedAt = createdAt
                });
            }
            else if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "You must provide a context if the DataPoints collection isn't valid.");
            }
            else if (context.Entry(this).IsKeySet)
            {
                context.Add(new DataPoint
                {
                    Value = value,
                    MetricId = Id,
                    CreatedAt = createdAt
                });
            }
            else
            {
                throw new InvalidOperationException("Could not add a new DataPoint");
            }
        }
    }
}