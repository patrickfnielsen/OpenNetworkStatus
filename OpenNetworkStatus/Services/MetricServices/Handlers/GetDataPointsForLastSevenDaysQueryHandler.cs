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
    public class GetDataPointsForLastSevenDaysQueryHandler : IRequestHandler<GetDataPointsForLastSevenDaysQuery, List<DataPointResource>>
    {
        private readonly StatusDataContext _dataContext;
        
        public GetDataPointsForLastSevenDaysQueryHandler(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<DataPointResource>> Handle(GetDataPointsForLastSevenDaysQuery request, CancellationToken cancellationToken)
        {
            //Note: We currently get all records for the last seven days
            //we might want to only get the records frome every 10, 20, or 30 minute - but that also depends on how often data is ingested!
            var dataPoints = await _dataContext.DataPoints
                .GetDataPointsForLast(day: 7, request.MetricId).AsNoTracking().ToListAsync(cancellationToken);

            return dataPoints.Select(DataPointResource.FromDataPoint).ToList();
        }
    }
}