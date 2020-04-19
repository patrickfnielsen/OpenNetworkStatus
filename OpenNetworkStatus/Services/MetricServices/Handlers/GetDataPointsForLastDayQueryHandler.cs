using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.MetricServices.Queries;
using OpenNetworkStatus.Services.MetricServices.Resources;

namespace OpenNetworkStatus.Services.MetricServices.Handlers
{
    public class GetDataPointsFOrLastDayQueryHandler : IRequestHandler<GetDataPointsForLastDayQuery, List<DataPointResource>>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetDataPointsFOrLastDayQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<DataPointResource>> Handle(GetDataPointsForLastDayQuery request, CancellationToken cancellationToken)
        {
            var dataPoints = await _dataContext.DataPoints
                .GetDataPointsLastDay(request.MetricId).AsNoTracking().ToListAsync(cancellationToken);

            return dataPoints.Select(DataPointResource.FromDataPoint).ToList();
        }
    }
}