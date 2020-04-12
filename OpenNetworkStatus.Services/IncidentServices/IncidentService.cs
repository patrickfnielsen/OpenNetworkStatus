using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Services.Exceptions;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices
{
    public class IncidentService
    {
        private readonly ILogger<IncidentService> _logger;
        private readonly StatusDataContext _dataContext;
        
        public IncidentService(ILogger<IncidentService> logger, StatusDataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        
        public async Task<GetIncidentResource> CreateAsync(AddIncidentResource incidentResource)
        {
            _logger.LogInformation("CreateAsync: {@incidentResource}", incidentResource);

            var incident = new Incident
            {
                Title = incidentResource.Title,
                Updates = new List<IncidentUpdate>
                {
                    new IncidentUpdate
                    {
                        Status = IncidentStatus.Investigating,
                        Message = incidentResource.Message
                    }
                }
            };
            
            _dataContext.Incidents.Add(incident);
            await _dataContext.SaveChangesAsync();

            return new GetIncidentResource
            {
                Id = incident.Id,
                Title = incident.Title,
                CreatedOn = incident.CreatedOn,
                UpdatedOn = incident.UpdatedOn
            };
        }
        
        public async Task<bool> DeleteIncidentAsync(int incidentId)
        {
            var incident = await _dataContext.Incidents.FindAsync(incidentId);
            if (incident == null)
            {
                _logger.LogDebug("Can't find incident with id: {id}", incidentId);

                throw new NotFoundException();
            }

            _logger.LogInformation("Delete Incident: {@incident}", incident);

            _dataContext.Incidents.Remove(incident);
            await _dataContext.SaveChangesAsync();

            return true;
        }
        
        public async Task<bool> UpdateIncidentAsync(int incidentId, UpdateIncidentResource incidentResource)
        {
            _logger.LogInformation("UpdateIncident: {@incidentResource}", incidentResource);

            var incident = new Incident
            {
                Id = incidentId,
                Title = incidentResource.Title
            };
                        
            _dataContext.Entry(incident).State = EntityState.Modified;
            
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!IncidentExists(incidentId))
            {
                _logger.LogDebug("Can't find incident with id: {id}", incidentId);

                throw new NotFoundException();
            }

            return true;
        }

        public async Task<GetIncidentResource> GetIncidentAsync(int incidentId)
        {
            _logger.LogInformation("Get Incident with id: {id}", incidentId);

            var incident = await _dataContext.Incidents.FindAsync(incidentId);
            if (incident == null)
            {
                _logger.LogDebug("Can't find incident with id: {id}", incidentId);
                return null;
            }

            return new GetIncidentResource
            {
                Id = incident.Id,
                Title = incident.Title,
                CreatedOn = incident.CreatedOn,
                UpdatedOn = incident.UpdatedOn
            };
        }
        
        public async Task<IEnumerable<GetIncidentResource>> GetIncidentsAsync(int page = 1, int limit = 50)
        {
            var result = await _dataContext.Incidents
                .Page(page, limit)
                .IncidentOrder()
                .ToListAsync();

            return result.Select(x => new GetIncidentResource
            {
                Id = x.Id,
                Title = x.Title,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn        
            });
        }

        public async Task<GetUpdateResource> GetIncidentUpdateAsync(int updateId)
        {
            _logger.LogInformation("Get Incident update with id: {id}", updateId);

            var update = await _dataContext.IncidentUpdates.FindAsync(updateId);
            if (update == null)
            {
                _logger.LogDebug("Can't find incident update with id: {id}", updateId);
                return null;
            }

            return new GetUpdateResource
            {
                Id = update.Id,
                Status = update.Status,
                Message = update.Message,
                CreatedOn = update.CreatedOn,
                UpdatedOn = update.UpdatedOn
            };
        }
        
        public async Task<IEnumerable<GetUpdateResource>> GetIncidentUpdatesAsync(int incidentId)
        {
            _logger.LogInformation("Get Incident updates from incident with id: {id}", incidentId);

            var incidentUpdates = await _dataContext.IncidentUpdates.Where(x => x.IncidentId == incidentId).ToListAsync();
            if (incidentUpdates == null)
            {
                _logger.LogDebug("Can't find updates for incident with with id: {id}", incidentId);
                return new List<GetUpdateResource>();
            }

            return incidentUpdates.Select(x => new GetUpdateResource
            {
                Id = x.Id,
                Status = x.Status,
                StatusText = StatusTextService.GetIncidentStatus(x.Status),
                Message = x.Message,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn
            });
        }
        
        public async Task<GetUpdateResource> AddIncidentUpdateAsync(int incidentId, AddUpdateResource updateResource)
        {
            var incident = await _dataContext.Incidents.FindAsync(incidentId);
            if (incident == null)
            {
                _logger.LogDebug("Can't find incident with id: {id}", incidentId);
                throw new NotFoundException();
            }
            
            _logger.LogInformation("AddIncidentUpdate: {@incidentUpdate}", updateResource);

            incident.AddUpdate(updateResource.Status, updateResource.Message, _dataContext);
            _dataContext.Incidents.Update(incident);

            await _dataContext.SaveChangesAsync();

            return new GetUpdateResource
            {
                Id = incident.Updates[0].Id,
                Status = updateResource.Status,
                Message = updateResource.Message,
                CreatedOn = incident.Updates[0].UpdatedOn,
                UpdatedOn = incident.Updates[0].CreatedOn
            };
        }
        
        private bool IncidentExists(long id) => _dataContext.Incidents.Any(e => e.Id == id);
    }
}