using System;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Services.Authentication.Resources
{
    public class UserResource
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public static UserResource FromUser(User user)
        {
            return new UserResource
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
