using System;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Data.Enums;

namespace OpenNetworkStatus.Services.IncidentServices.Resources
{
    public class IncidentUpdateResource
    {
        public int Id { get; set; }

        public int IncidentId { get; set; }

        public IncidentStatus Status { get; set; }

        public string StatusText { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        public static IncidentUpdateResource FromIncidentUpdate(IncidentUpdate update)
        {
            if (update == null)
            {
                return null;
            }

            return new IncidentUpdateResource
            {
                Id = update.Id,
                IncidentId = update.IncidentId,
                Status = update.Status,
                StatusText = EnumTextService.GetIncidentStatus(update.Status),
                Message = update.Message,
                CreatedAt = update.CreatedAt,
                UpdatedAt = update.UpdatedAt
            };
        }
    }
}