using System;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.MetricServices.Resources
{
    public class DataPointResource
    {
        public double Value { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public static DataPointResource FromDataPoint(DataPoint data)
        {
            return new DataPointResource
            {
                Value = data.Value,
                CreatedAt = data.CreatedAt
            };
        }
    }
}