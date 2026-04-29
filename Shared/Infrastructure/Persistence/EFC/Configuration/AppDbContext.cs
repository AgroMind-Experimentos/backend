using EcotrackPlatform.API.Organization.Domain.Model.Aggregates;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using EcotrackPlatform.API.Profile.Infrastructure.Persistence.EFC.Configuration.Extensions;

using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Aggregates;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Model.Entities;
using EcotrackPlatform.API.Organization.Domain.Model.Entities;
using MonitoringExtensions = EcotrackPlatform.API.Monitoringandcontrol.Infraestructure.Persistence.EFC.Extensions.ModelBuilderExtensions;
using EcotrackPlatform.API.Report.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TaskAggregate> Tasks { get; set; }
    public DbSet<Checklist> Checklists { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }
    public DbSet<Logbook> Logbooks { get; set; }
    public DbSet<Organization.Domain.Model.Aggregates.Organization> Organizations { get; set; }
    public DbSet<Crop> Crops { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }
    public DbSet<CropMember> CropMembers { get; set; }

    // Report Module
    public DbSet<EcotrackPlatform.API.Report.Domain.Model.Report> Reports { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Automatically set CreatedDate and UpdatedDate for entities
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

        // --- DbSets mínimos (ajusta con tus entidades reales) ---
        public DbSet<EcotrackPlatform.API.Profile.Domain.Model.Aggregates.Profile> Profiles => Set<EcotrackPlatform.API.Profile.Domain.Model.Aggregates.Profile>();
        public DbSet<EcotrackPlatform.API.Profile.Domain.Model.Aggregates.ProfileSettings> ProfileSettings => Set<EcotrackPlatform.API.Profile.Domain.Model.Aggregates.ProfileSettings>();

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

            builder.Entity<CropMember>()
                .HasKey(cm => new { cm.ProfileId, cm.CropId });

            builder.Entity<CropMember>()
                .HasOne(cm => cm.Profile)
                .WithMany()
                .HasForeignKey(cm => cm.ProfileId);

            builder.Entity<Crop>()
                .HasMany(c => c.Members)
                .WithOne(cm => cm.Crop)
                .HasForeignKey(cm => cm.CropId);
        
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
