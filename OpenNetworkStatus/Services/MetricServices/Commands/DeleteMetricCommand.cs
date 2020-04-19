using System.ComponentModel.DataAnnotations;
using MediatR;

namespace OpenNetworkStatus.Services.MetricServices.Commands
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class DeleteMetricCommand : IRequest<bool>
    {
        [Required]
        public int Id { get; set; }
    }
}