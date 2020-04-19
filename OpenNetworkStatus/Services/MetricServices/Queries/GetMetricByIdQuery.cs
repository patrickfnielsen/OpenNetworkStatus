using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetMetricByIdQuery : IRequest<MetricResource>
    {
        [Required]
        public int Id { get; set; }
    }
}