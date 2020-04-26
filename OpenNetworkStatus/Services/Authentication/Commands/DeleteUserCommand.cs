using MediatR;

namespace OpenNetworkStatus.Services.Authentication.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
