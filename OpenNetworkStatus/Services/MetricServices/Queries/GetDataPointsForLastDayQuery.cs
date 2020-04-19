using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Queries
{
    public class GetDataPointsForLastDayQuery : IRequest<List<DataPointResource>>
    {
        [Required]
        public int MetricId { get; set; }
    }
}