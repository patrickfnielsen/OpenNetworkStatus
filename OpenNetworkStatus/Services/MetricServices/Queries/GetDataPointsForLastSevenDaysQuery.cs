using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Queries
{
    public class GetDataPointsForLastSevenDaysQuery : IRequest<List<DataPointResource>>
    {
        [Required]
        public int MetricId { get; set; }
    }
}