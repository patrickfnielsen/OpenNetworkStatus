using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Services.ComponentServices.Commands;
using OpenNetworkStatus.Services.MetricServices.Commands;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class DeleteMetricCommandHandler : IRequestHandler<DeleteMetricCommand, bool>
    {
        private readonly StatusDataContext _dataContext;
        
        public DeleteMetricCommandHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> Handle(DeleteMetricCommand request, CancellationToken cancellationToken)
        {
            var metric = await _dataContext.Metrics.FindAsync(request.Id);
            if (metric == null)
            {
                return false;
            }

            _dataContext.Metrics.Remove(metric);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}