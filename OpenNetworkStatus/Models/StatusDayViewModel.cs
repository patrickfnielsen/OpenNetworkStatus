using System;
using System.Collections.Generic;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Models
{
    public class StatusDayViewModel
    {
        public StatusDayViewModel(DateTime date, IEnumerable<Incident> incidents)
        {
            Date = date;
            Incidents = incidents;
        }

        public DateTime Date { get; }
        
        public IEnumerable<Incident> Incidents { get; }
    }
}