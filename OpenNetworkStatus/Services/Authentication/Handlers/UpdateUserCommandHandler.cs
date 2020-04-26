using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenNetworkStatus.Data;
using OpenNetworkStatus.Data.Entities;
using OpenNetworkStatus.Services.Authentication.Commands;
using OpenNetworkStatus.Services.Authentication.Resources;

namespace OpenNetworkStatus.Services.Authentication.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResource>
    {
        private readonly StatusDataContext _dataContext;
        private readonly PasswordHasher _passwordHasher;

        public UpdateUserCommandHandler(StatusDataContext dataContext, PasswordHasher passwordHasher)
        {
            _dataContext = dataContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResource> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = request.Id,
                Username = request.Username,
                PasswordHash = _passwordHasher.HashPassword(request.Password)
            };

            _dataContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!ComponentExists(request.Id))
            {
                return null;
            }

            return UserResource.FromUser(user);
        }

        private bool ComponentExists(int id) => _dataContext.Users.Any(e => e.Id == id);
    }
}
