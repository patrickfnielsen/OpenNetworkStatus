using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OpenNetworkStatus.Data
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<StatusDataContext>();
            if (appContext.Database.GetPendingMigrations().Any())
            {
                //TODO: Log this
                appContext.Database.Migrate();
            }

            return host;
        }
    }
}