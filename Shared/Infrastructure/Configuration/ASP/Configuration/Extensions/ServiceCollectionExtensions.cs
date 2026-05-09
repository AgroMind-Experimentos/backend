using System.Text;
using EcotrackPlatform.API.Iam.Application.Internal.CommandServices;
using EcotrackPlatform.API.Iam.Domain.Repositories;
using EcotrackPlatform.API.Iam.Domain.Services;
using EcotrackPlatform.API.Iam.Infrastructure.Repositories;
using EcotrackPlatform.API.Iam.Infrastructure.Tokens;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.CommandServices;
using EcotrackPlatform.API.Monitoringandcontrol.Application.Internal.QueryServices;
using EcotrackPlatform.API.Monitoringandcontrol.Domain.Repositories;
using EcotrackPlatform.API.Monitoringandcontrol.Infraestructure.Persistence.EFC.Respositories;
using EcotrackPlatform.API.Profile.Application.Internal.CommandServices;
using EcotrackPlatform.API.Profile.Application.Internal.QueryServices;
using EcotrackPlatform.API.Profile.Domain.Repositories;
using EcotrackPlatform.API.Profile.Infrastructure.Repositories;
using EcotrackPlatform.API.Shared.Domain.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.Connection;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using EcotrackPlatform.API.Shared.Infrastructure.Persistence.Repositories;
using EcotrackPlatform.API.Shared.Infrastructure.Security.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EcotrackPlatform.API.Shared.Infrastructure.Configuration.ASP.Configuration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRoutingAndControllers(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllers(options =>
        {
            options.Conventions.Add(new KebabCaseRouteNamingConvention());
            options.RespectBrowserAcceptHeader = true;
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        })
        .AddXmlSerializerFormatters()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressConsumesConstraintForFormFileParameters = true;
        });

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllPolicy", policy =>
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        return services;
    }

    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IWebHostEnvironment environment)
    {
        try 
        {
            var connectionString = DbConnectionStringLoader.GetConnectionString(environment.IsProduction());

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(connectionString);

                if (environment.IsDevelopment())
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging();
                }
                else
                {
                    options.LogTo(Console.WriteLine, LogLevel.Error)
                        .EnableDetailedErrors();
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] Failed to configure Database: {ex.Message}");
            throw;
        }

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = JwtAuthConfigurationLoader.Load(configuration);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidAudience = jwtConfiguration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey))
                };
            });
        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Agromind Ecotrack Platform API",
                Version = "v1",
                Description = "Backend RESTful API for Agromind Ecotrack"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

public static IServiceCollection AddSharedContext(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static IServiceCollection AddMonitoringAndControlContext(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IChecklistRepository, ChecklistRepository>();
        services.AddScoped<ILogbookRepository, LogbookRepository>();
        services.AddScoped<CreateTaskCommandService>();
        services.AddScoped<UpdateTaskCommandService>();
        services.AddScoped<CreateChecklistCommandService>();
        services.AddScoped<UpdateChecklistCommandService>();
        services.AddScoped<UpdateChecklistItemCommandService>();
        services.AddScoped<UpdateTaskStatusCommandService>();
        services.AddScoped<DeleteTaskCommandService>();
        services.AddScoped<GetTasksQueryService>();
        services.AddScoped<GetChecklistByTaskIdQueryService>();
        services.AddScoped<CreateLogbookCommandService>();
        services.AddScoped<GetLogbookQueryService>();
        return services;
    }

    public static IServiceCollection AddReportContext(this IServiceCollection services)
    {
        services.AddScoped<Report.Application.Internal.QueryServices.ReportQueryService>();
        services.AddScoped<Report.Infrastructure.Services.PdfReportGeneratorService>();
        return services;
    }

    public static IServiceCollection AddOrganizationContext(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<Organization.Domain.Repositories.IOrganizationRepository,
            Organization.Infrastructure.Repositories.OrganizationRepository>();
        services.AddScoped<Organization.Domain.Repositories.ICropRepository,
            Organization.Infrastructure.Repositories.CropRepository>();
        services.AddScoped<Organization.Domain.Repositories.IInvitationRepository,
            Organization.Infrastructure.Repositories.InvitationRepository>();

        // Services
        services.AddScoped<Organization.Aplication.Services.IOrganizationCommandService,
            Organization.Aplication.Internal.CommandServices.OrganizationCommandService>();
        services.AddScoped<Organization.Aplication.Services.IOrganizationQueryService,
            Organization.Aplication.Internal.QueryServices.OrganizationQueryService>();
        services.AddScoped<Organization.Aplication.Services.ICropCommandService,
            Organization.Aplication.Internal.CommandServices.CropCommandService>();
        services.AddScoped<Organization.Aplication.Services.ICropQueryService,
            Organization.Aplication.Internal.QueryServices.CropQueryService>();
        services.AddScoped<Organization.Aplication.Internal.CommandServices.InvitationCommandService>();
        return services;
    }

    public static IServiceCollection AddIamContext(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<AuthCommandService>();
        services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        return services;
    }

    public static IServiceCollection AddProfileContext(this IServiceCollection services)
    {
        services.AddScoped<ProfileQueryService>();
        services.AddScoped<ProfileCommandService>();
        services.AddScoped<IProfileSettingsRepository, ProfileSettingsRepository>();
        services.AddScoped<SettingsQueryService>();
        services.AddScoped<SettingsCommandService>();
        return services;
    }
}