using System.ComponentModel.DataAnnotations;
using MediatR;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Commands
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class AddMetricCommand : IRequest<MetricResource>
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Suffix { get; set; }

        public string Description { get; set; }
        
        public int Position { get; set; }
    }
}