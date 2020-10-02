using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Queries
{
    public class GetDataPointsForDayIntervalQuery : IRequest<List<DataPointResource>>
    {
        [Required]
        public int MetricId { get; set; }

        public int Interval { get; set; } = 300;

        public int Days { get; set; } = 1;
    }
}