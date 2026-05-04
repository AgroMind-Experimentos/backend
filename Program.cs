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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Setup builder reading environment variables and inline args
var builder = WebApplication.CreateBuilder(args);

// Secure Database Connection String
var connectionString = DbConnectionStringLoader.GetConnectionString(builder.Environment.IsProduction());

// Add Configuration for Routing
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new KebabCaseRouteNamingConvention());
    options.RespectBrowserAcceptHeader = true;
})
.AddXmlSerializerFormatters()
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressConsumesConstraintForFormFileParameters = true;
});

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure Database Context and Logging Levels
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
    }
    else if (builder.Environment.IsProduction())
    {
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
    }
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// Configure Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

// Dependency Injection

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Monitoring And Control Bounded Context
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IChecklistRepository, ChecklistRepository>();
builder.Services.AddScoped<ILogbookRepository, LogbookRepository>();
builder.Services.AddScoped<CreateTaskCommandService>();
builder.Services.AddScoped<CreateChecklistCommandService>();
builder.Services.AddScoped<UpdateTaskStatusCommandService>();
builder.Services.AddScoped<GetTasksQueryService>();
builder.Services.AddScoped<GetChecklistByTaskIdQueryService>();
builder.Services.AddScoped<CreateLogbookCommandService>();
builder.Services.AddScoped<GetLogbookQueryService>();

// Report Bounded Context
builder.Services.AddScoped<EcotrackPlatform.API.Report.Application.Internal.QueryServices.ReportQueryService>();
builder.Services.AddScoped<EcotrackPlatform.API.Report.Infrastructure.Services.PdfReportGeneratorService>();

builder.Services.AddScoped<EcotrackPlatform.API.Organization.Domain.Repositories.IOrganizationRepository,
    EcotrackPlatform.API.Organization.Infrastructure.Repositories.OrganizationRepository>();
builder.Services.AddScoped<EcotrackPlatform.API.Organization.Domain.Repositories.ICropRepository,
    EcotrackPlatform.API.Organization.Infrastructure.Repositories.CropRepository>();

// Organization Context - Services
builder.Services.AddScoped<EcotrackPlatform.API.Organization.Aplication.Services.IOrganizationCommandService,
    EcotrackPlatform.API.Organization.Aplication.Internal.CommandServices.OrganizationCommandService>();
builder.Services.AddScoped<EcotrackPlatform.API.Organization.Aplication.Services.IOrganizationQueryService,
    EcotrackPlatform.API.Organization.Aplication.Internal.QueryServices.OrganizationQueryService>();
builder.Services.AddScoped<EcotrackPlatform.API.Organization.Aplication.Services.ICropCommandService,
    EcotrackPlatform.API.Organization.Aplication.Internal.CommandServices.CropCommandService>();
builder.Services.AddScoped<EcotrackPlatform.API.Organization.Aplication.Services.ICropQueryService,
    EcotrackPlatform.API.Organization.Aplication.Internal.QueryServices.CropQueryService>();

// IAM Bounded Context
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<AuthCommandService>();
builder.Services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

// Profile Bounded Context
builder.Services.AddScoped<ProfileQueryService>();
builder.Services.AddScoped<ProfileCommandService>();
builder.Services.AddScoped<IProfileSettingsRepository, ProfileSettingsRepository>();
builder.Services.AddScoped<SettingsQueryService>();
builder.Services.AddScoped<SettingsCommandService>();

var app = builder.Build();

// Verify Database Objects are Created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS
app.UseCors("AllowAllPolicy");

// HTTPS Redirection
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
