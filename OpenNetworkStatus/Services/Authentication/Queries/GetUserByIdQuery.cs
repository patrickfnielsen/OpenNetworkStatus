using MediatR;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.Authentication.Queries
{
    //TODO: Private setters when System.Text.json 5.0 is released
    public class GetUserByIdQuery : IRequest<UserResource>
    {
        public int Id { get; set; }
    }
}
