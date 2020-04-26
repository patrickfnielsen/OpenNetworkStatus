using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.Authentication;
using OpenNetworkStatus.Services.Authentication.Commands;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.ComponentGroupServices.Handlers
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, AddUserResource>
    {
        private readonly StatusDataContext _dataContext;
        private readonly PasswordHasher _passwordHasher;

        public AddUserCommandHandler(StatusDataContext userService, PasswordHasher passwordHasher)
        {
            _dataContext = userService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AddUserResource> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (_dataContext.Users.Any(x => x.Username == request.Username))
            {
                return new AddUserResource(null, "Username is already taken");
            }

            var user = new User()
            {
                Username = request.Username
            };

            user.PasswordHash = _passwordHasher.HashPassword(request.Password);

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return new AddUserResource(UserResource.FromUser(user));
        }
    }
}