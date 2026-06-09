using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;
using EcotrackPlatform.API.Organizations.Domain.Model.Aggregates;
using EcotrackPlatform.API.Organizations.Domain.Model.Entities;
using EcotrackPlatform.API.Profiles.Domain.Model.Aggregates;
using EcotrackPlatform.API.Profiles.Infrastructure.Persistence.EFC.Configuration.Extensions;
using MonitoringExtensions = EcotrackPlatform.API.Monitoringandcontrol.Infraestructure.Persistence.EFC.Extensions.ModelBuilderExtensions;
using EcotrackPlatform.API.Report.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TaskAggregate> Tasks { get; set; }
    public DbSet<Checklist> Checklists { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }
    public DbSet<Logbook> Logbooks { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Plot> Plots { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }
    public DbSet<Invitation> Invitations { get; set; }

    // Report Module
    public DbSet<EcotrackPlatform.API.Report.Domain.Model.Report> Reports { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<ProfileSettings> ProfileSettings => Set<ProfileSettings>();
        public DbSet<EcotrackPlatform.API.Iam.Domain.Model.Aggregates.AuthSession> AuthSessions => Set<EcotrackPlatform.API.Iam.Domain.Model.Aggregates.AuthSession>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Owned<EcotrackPlatform.API.Organizations.Domain.Model.ValueObjects.Coordinates>();

            builder.Entity<Organization>(entity =>
            {
                entity.OwnsOne(o => o.Coordinates, coordinates =>
                {
                    coordinates.WithOwner().HasForeignKey("Id");

                    coordinates.Property(c => c.Latitude)
                        .IsRequired();
    
                    coordinates.Property(c => c.Longitude)
                        .IsRequired();
                });

                entity.HasOne(o => o.Profile)
                    .WithMany(p => p.Organizations)
                    .HasForeignKey(o => o.AgronomistOwnerId);
            });
            
            builder.Entity<OrganizationMember>()
                .HasKey(om => new { om.ProfileId, om.OrganizationId });

            builder.Entity<OrganizationMember>()
                .HasOne(om => om.Profile)
                .WithMany(p => p.Memberships)
                .HasForeignKey(om => om.ProfileId);

            builder.Entity<OrganizationMember>()
                .HasOne(om => om.Organization)
                .WithMany(o => o.Members)
                .HasForeignKey(om => om.OrganizationId);

            builder.Entity<Plot>()
                .HasKey(p => p.Id);
        
            builder.Entity<ChecklistItem>()
                .HasOne<Checklist>()
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.ChecklistId);

            builder.AddProfileModule();
            builder.AddReportModule();
            MonitoringExtensions.ApplyConfigurations(builder);
            builder.UseSnakeCaseNamingConvention();
        }
}
