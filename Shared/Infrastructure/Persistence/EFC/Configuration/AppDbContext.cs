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
    public DbSet<PlotMember> PlotMembers { get; set; }
    public DbSet<Invitation> Invitations { get; set; }

    // Report Module
    public DbSet<EcotrackPlatform.API.Report.Domain.Model.Report> Reports { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Automatically set CreatedDate and UpdatedDate for entities
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

        // --- DbSets mínimos (ajusta con tus entidades reales) ---
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<ProfileSettings> ProfileSettings => Set<ProfileSettings>();

        // Si ya tienes AuthSession en Iam, puedes exponerlo:
        public DbSet<EcotrackPlatform.API.Iam.Domain.Model.Aggregates.AuthSession> AuthSessions => Set<EcotrackPlatform.API.Iam.Domain.Model.Aggregates.AuthSession>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
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

            builder.Entity<PlotMember>()
                .HasKey(cm => new { cm.ProfileId, cm.PlotId });

            builder.Entity<PlotMember>()
                .HasOne(cm => cm.Profile)
                .WithMany()
                .HasForeignKey(cm => cm.ProfileId);

            builder.Entity<Plot>()
                .HasMany(c => c.Members)
                .WithOne(cm => cm.Plot)
                .HasForeignKey(cm => cm.PlotId);
        
            builder.Entity<ChecklistItem>()
                .HasOne<Checklist>()
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.ChecklistId);

            // Módulos por bounded context
            builder.AddProfileModule();
            
            // Aplicar configuraciones del módulo Report
            builder.AddReportModule();
            
            // Aplicar configuraciones del módulo Monitoringandcontrol
            MonitoringExtensions.ApplyConfigurations(builder);
            
            // Apply naming convention to use snake_case for database objects
            builder.UseSnakeCaseNamingConvention();
        }
}
