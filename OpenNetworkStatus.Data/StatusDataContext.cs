using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OpenNetworkStatus.Data.Entities;

namespace OpenNetworkStatus.Data
{    
    public class StatusDataContext : DbContext
    {
        public StatusDataContext(DbContextOptions<StatusDataContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ComponentGroup>(builder =>
            {
                builder.Property(e => e.CreatedOn).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<Component>(builder =>
            {
                builder.Property(e => e.CreatedOn).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<Metric>(builder =>
            {
                builder.Property(e => e.CreatedOn).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
            
            modelBuilder.Entity<Incident>(builder =>
            {
                builder.Property(e => e.CreatedOn).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
            
            modelBuilder.Entity<IncidentUpdate>(builder =>
            {
                builder.Property(e => e.CreatedOn).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
        }
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSave()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (!(entry.Entity is AuditEntityBase audit)) continue;
                
                var now = DateTime.UtcNow;
                if (entry.State == EntityState.Modified)
                {
                    audit.UpdatedOn = now;
                } 
                else if (entry.State == EntityState.Added)
                {
                    audit.CreatedOn = now;
                    audit.UpdatedOn = now;
                }
            }
        }
        
        public DbSet<ComponentGroup> ComponentGroups { get; set; }
        
        public DbSet<Component> Components { get; set; }

        public DbSet<Metric> Metrics { get; set; }
        
        public DbSet<DataPoint> DataPoints { get; set; }
        
        public DbSet<Incident> Incidents { get; set; }
        
        public DbSet<IncidentUpdate> IncidentUpdates { get; set; }
    }
}