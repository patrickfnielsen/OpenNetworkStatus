using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNetworkStatus.Data.Entities
{
    public class DataPoint
    {
        public int Id { get; set; }
        
        public int MetricId { get; set; }
        
        [Required]
        public double Value { get; set; }
        
        [Required]
        public DateTime CreatedOn { get; set; }
    }
}