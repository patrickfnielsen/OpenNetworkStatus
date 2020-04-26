using MediatR;

namespace OpenNetworkStatus.Services.IncidentServices.Commands
{
    public class DeleteIncidentCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
