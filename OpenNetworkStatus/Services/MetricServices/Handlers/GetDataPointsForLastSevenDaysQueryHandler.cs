using System;
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
            //Note: We currently get all records for the last seven days, and then computre what records to display below
            var dataPoints = await _dataContext.DataPoints
                .GetDataPointsForLast(day: 7, request.MetricId).AsNoTracking().ToListAsync(cancellationToken);

            //Todo: This must be possible to do directly in the database.
            var ticks = TimeSpan.FromMinutes(30).Ticks;
            dataPoints = dataPoints.GroupBy(s => s.CreatedAt.Ticks / ticks)
                .Select(s => s.First()).ToList();

            return dataPoints.Select(DataPointResource.FromDataPoint).ToList();
        }
    }
}