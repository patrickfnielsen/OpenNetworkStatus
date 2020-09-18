using System;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.MetricServices.Resources
{
    public class DataPointResource
    {
        public double? Value { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public static DataPointResource FromDataPoint(DataPoint data)
        {
            double? value = null;
            if (data.Value != 0)
            {
                value = data.Value;
            }

            return new DataPointResource
            {
                Value = value,
                CreatedAt = data.CreatedAt
            };
        }
    }
}