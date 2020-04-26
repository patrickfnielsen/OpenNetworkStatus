using MediatR;

namespace OpenNetworkStatus.Services.IncidentServices.Commands
{
    public class DeleteIncidentUpdateCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
