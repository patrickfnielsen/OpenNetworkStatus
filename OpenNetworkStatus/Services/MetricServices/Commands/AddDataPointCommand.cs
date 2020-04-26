using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Commands
{
    public class AddDataPointCommand : IRequest<DataPointResource>
    {        
        internal int MetricId { get; set; }
        
        [Required]
        public double Value { get; set; }
        
        public DateTime? CreatedAt { get; set; } 
    }
}