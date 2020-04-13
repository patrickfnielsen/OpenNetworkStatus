using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Models;
using OpenNetworkStatus.Services.StatusServices;

namespace OpenNetworkStatus.Controllers
{
    public class StatusController : Controller
    {
        private readonly StatusDataContext _dataContext;
        private readonly SiteStatusCalculationService _siteStatusCalculation;
        private readonly StatusDayGenerationService _statusDayGeneration;

        public StatusController(StatusDataContext dataContext, SiteStatusCalculationService siteStatusCalculation, StatusDayGenerationService statusDayGeneration)
        {
            _dataContext = dataContext;
            _siteStatusCalculation = siteStatusCalculation;
            _statusDayGeneration = statusDayGeneration;
        }

        public async Task<IActionResult> Index()
        {
            var componentGroups = await GetComponentGroups();
            var allComponents = await GetComponents();
            var metrics = await GetMetrics();
            var statusDays = await _statusDayGeneration.GetStatusForLastDaysAsync(7);
            var siteStatus = GetSiteStatus(allComponents);

            var standaloneComponents = allComponents.Where(x => x.ComponentGroupId == null);
            var vm = new StatusViewModel(siteStatus, componentGroups, standaloneComponents, metrics, statusDays);
            
            return View(vm);
        }

        private async Task<List<ComponentGroup>> GetComponentGroups()
        {
            return await _dataContext.ComponentGroups
                .Include(x => x.Components)
                .ComponentGroupOrder()
                .ToListAsync();
        }
        
        private async Task<List<Component>> GetComponents()
        {
            return await _dataContext.Components
                .ComponentOrder()
                .ToListAsync();
        }
        
        private async Task<List<Metric>> GetMetrics()
        {
            return await _dataContext.Metrics
                .MetricOrder()
                .ToListAsync();
        }

        private SiteStatusViewModel GetSiteStatus(List<Component> components)
        {
            var siteStatus = _siteStatusCalculation.CalculateSiteStatus(components);

            return new SiteStatusViewModel(siteStatus);
        }
    }
}