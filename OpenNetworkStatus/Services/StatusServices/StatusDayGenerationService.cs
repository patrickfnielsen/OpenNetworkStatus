using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Models;

namespace OpenNetworkStatus.Services.StatusServices
{
    public class StatusDayGenerationService
    {
        private readonly StatusDataContext _dataContext;

        public StatusDayGenerationService(StatusDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<StatusDayViewModel>> GetStatusForLastDaysAsync(int days)
        {
            var incidentsInPeriod = await _dataContext.Incidents.GetIncidentsLast(days).ToListAsync();
            
            var statusDays = new List<StatusDayViewModel>();
            for (var i = 0; i <= days; i++)
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                var incidents = incidentsInPeriod
                    .Where(x => x.CreatedOn.Date == date);
                
                statusDays.Add(new StatusDayViewModel(date, incidents));
            }

            return statusDays;
        }
    }
}