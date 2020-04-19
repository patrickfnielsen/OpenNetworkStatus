using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.Enums;
using OpenNetworkStatus.Data.QueryObjects;
using OpenNetworkStatus.Exceptions;
using OpenNetworkStatus.Services.IncidentServices.Resources;

namespace OpenNetworkStatus.Services.IncidentServices
{
    public class IncidentService
    {
        private readonly ILogger<IncidentService> _logger;
        private readonly StatusDataContext _dataContext;
        private readonly IncidentMapper _mapper;
        
        public IncidentService(ILogger<IncidentService> logger, StatusDataContext dataContext, IncidentMapper mapper)
        {
            _logger = logger;
            _dataContext = dataContext;
            _mapper = mapper;
        }
        
        public async Task<GetIncidentResource> CreateAsync(AddIncidentResource incidentResource)
        {
            _logger.LogDebug("CreateAsync: {@incidentResource}", incidentResource);

            var incident = new Incident
            {
                Title = incidentResource.Title,
                Impact = incidentResource.Impact,
                ResolvedAt = incidentResource.ResolvedAt
            };

            _dataContext.Incidents.Add(incident);
            await _dataContext.SaveChangesAsync();
            
            return _mapper.ToIncidentResource(incident);
        }
        
        public async Task DeleteIncidentAsync(int incidentId)
        {
            var incident = await _dataContext.Incidents.FindAsync(incidentId);
            if (incident == null)
            {
                _logger.LogWarning("Can't find incident with id: {id}", incidentId);

                throw new NotFoundException();
            }

            _logger.LogDebug("DeleteIncidentAsync: {@incident}", incident);

            _dataContext.Incidents.Remove(incident);
            await _dataContext.SaveChangesAsync();
        }
        
        public async Task UpdateIncidentAsync(int incidentId, AddIncidentResource incidentResource)
        {
            _logger.LogDebug("UpdateIncidentAsync: {@incidentResource}", incidentResource);

            var incident = new Incident
            {
                Id = incidentId,
                Impact = incidentResource.Impact,
                Title = incidentResource.Title,
                ResolvedAt = incidentResource.ResolvedAt
            };
            
            _dataContext.Entry(incident).State = EntityState.Modified;
            
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!IncidentExists(incidentId))
            {
                _logger.LogWarning("Can't find incident with id: {id}", incidentId);

                throw new NotFoundException();
            }
        }

        public async Task<GetIncidentResource> GetIncidentAsync(int incidentId)
        {
            _logger.LogDebug("GetIncidentAsync: {id}", incidentId);

            var incident = await _dataContext.Incidents
                .Include(x => x.Updates)
                .SingleOrDefaultAsync(x => x.Id == incidentId);
            
            if (incident == null)
            {
                _logger.LogWarning("Can't find incident with id: {id}", incidentId);
                return null;
            }

            return _mapper.ToIncidentResource(incident);
        }
        
        public async Task<IEnumerable<GetIncidentResource>> GetIncidentsAsync(int page, int limit)
        {
            var incidents = await _dataContext.Incidents
                .Include(x => x.Updates)
                .Page(page, limit)
                .IncidentOrder()
                .AsNoTracking()
                .ToListAsync();

            return _mapper.ToIncidentResource(incidents);
        }

        public async Task<GetIncidentUpdateResource> AddIncidentUpdateAsync(int incidentId, AddIncidentUpdateResource updateResource)
        {
            var incident = await _dataContext.Incidents.FindAsync(incidentId);
            if (incident == null)
            {
                _logger.LogWarning("Can't find incident with id: {id}", incidentId);
                throw new NotFoundException();
            }
            
            _logger.LogDebug("AddIncidentUpdate: {@incidentUpdate}", updateResource);

            //If the update says the issue is resolved, update the resolved date on the incident
            if (updateResource.Status == IncidentStatus.Resolved)
            {
                incident.ResolvedAt = DateTime.UtcNow;
            }

            //If the incidents resolved date is set, but the current sate is not resolved (ie if we backup because the issue is not fixed)
            //The set the resolve date to null
            if (incident.ResolvedAt != null && updateResource.Status != IncidentStatus.Resolved)
            {
                incident.ResolvedAt = null;
            }
            
            incident.AddUpdate(updateResource.Status, updateResource.Message, _dataContext);
            _dataContext.Incidents.Update(incident);

            await _dataContext.SaveChangesAsync();

            return _mapper.ToIncidentUpdateResource(incident.Updates[0]);
        }

        public async Task UpdateIncidentUpdateAsync(int incidentId, int updateId, AddIncidentUpdateResource updateResource)
        {
            _logger.LogDebug("UpdateIncident: {@incidentResource}", updateResource);

            var incident = new IncidentUpdate
            {
                Id = updateId,
                IncidentId = incidentId,
                Status = updateResource.Status,
                Message = updateResource.Message
            };
                        
            _dataContext.Entry(incident).State = EntityState.Modified;
            
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!IncidentExists(incidentId))
            {
                _logger.LogWarning("Can't find incident with id: {id}", incidentId);

                throw new NotFoundException();
            }
        }

        public async Task<GetIncidentUpdateResource> GetIncidentUpdateAsync(int incidentId, int updateId)
        {
            _logger.LogDebug("GetIncidentUpdateAsync: {id}", updateId);

            var update = await _dataContext.IncidentUpdates
                .Where(x => x.IncidentId == incidentId && x.Id == updateId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
            
            if (update == null)
            {
                _logger.LogWarning("Can't find incident update with id: {id}", updateId);
                return null;
            }

            return _mapper.ToIncidentUpdateResource(update);
        }
        
        public async Task<List<GetIncidentUpdateResource>> GetIncidentUpdatesAsync(int incidentId, int page, int limit)
        {
            _logger.LogDebug("GetIncidentUpdatesAsync: {id}", incidentId);

            var updates = await _dataContext.IncidentUpdates
                .Where(x => x.IncidentId == incidentId)
                .Page(page, limit)
                .AsNoTracking()
                .ToListAsync();
            
            if (updates == null)
            {
                _logger.LogWarning("Can't find incident updates for incident with id: {id}", incidentId);
                return new List<GetIncidentUpdateResource>();
            }

            return _mapper.ToIncidentUpdateResource(updates);
        }
                
        private bool IncidentExists(long id) => _dataContext.Incidents.Any(e => e.Id == id);
    }
}