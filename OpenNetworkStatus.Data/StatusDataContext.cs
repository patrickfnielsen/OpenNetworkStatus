using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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


            IgnoreCreatedAtOnUpdates(modelBuilder);
            ForceUtcDateTime(modelBuilder);
            CreateDefaultAdminAccount(modelBuilder);
        }


        private void CreateDefaultAdminAccount(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "50DcN6mhtLk8FE1Bwy6N0fUHKG8uCgyZkjtUYCSBDZhHRHxBg8ywotiQ3Zt6i6rDAA==", //Equals to "password"
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }

        private void IgnoreCreatedAtOnUpdates(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComponentGroup>(builder =>
            {
                builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<Component>(builder =>
            {
                builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<Metric>(builder =>
            {
                builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
            
            modelBuilder.Entity<Incident>(builder =>
            {
                builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
            
            modelBuilder.Entity<IncidentUpdate>(builder =>
            {
                builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<User>(builder =>
            {
                builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                builder.Property(e => e.Id).HasIdentityOptions(startValue: 2);
            });
        }

        private void ForceUtcDateTime(ModelBuilder modelBuilder)
        {
            //All dates SHOULD already be in UTC, but just in case we force them to UTC to keep the database consistent
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                }
            }
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
            //Set UpdatedAt + CreatedAt dates
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (!(entry.Entity is AuditEntityBase audit)) continue;
                
                var now = DateTime.UtcNow;
                if (entry.State == EntityState.Modified)
                {
                    audit.UpdatedAt = now;
                } 
                else if (entry.State == EntityState.Added)
                {
                    audit.CreatedAt = now;
                    audit.UpdatedAt = now;
                }
            }
        }
        
        public DbSet<ComponentGroup> ComponentGroups { get; set; }
        
        public DbSet<Component> Components { get; set; }

        public DbSet<Metric> Metrics { get; set; }
        
        public DbSet<DataPoint> DataPoints { get; set; }
        
        public DbSet<Incident> Incidents { get; set; }
        
        public DbSet<IncidentUpdate> IncidentUpdates { get; set; }

        public DbSet<User> Users { get; set; }
    }
}