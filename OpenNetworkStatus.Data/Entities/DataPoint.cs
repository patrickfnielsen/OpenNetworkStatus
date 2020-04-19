using System;

namespace OpenNetworkStatus.Data.Entities
{
    public class DataPoint
    {
        public int Id { get; set; }
        
        public int MetricId { get; set; }
        
        public double Value { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}