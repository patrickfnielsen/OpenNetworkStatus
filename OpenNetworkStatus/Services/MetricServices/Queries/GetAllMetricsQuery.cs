using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetAllMetricsQuery : IRequest<List<MetricResource>>
    {
        [Required]
        public int Page { get; set; }
        
        [Required]
        public int Limit { get; set; }

        public GetAllMetricsQuery()
        {
            Page = 1;
            Limit = 50;
        }
    }
}