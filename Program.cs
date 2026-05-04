using EcotrackPlatform.API.Shared.Infrastructure.Configuration.ASP.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRoutingAndControllers()
    .AddCorsConfiguration()
    .AddDatabaseConfiguration(builder.Environment)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddSharedContext()
    .AddIamContext()
    .AddProfileContext()
    .AddOrganizationContext()
    .AddMonitoringAndControlContext()
    .AddReportContext();

var app = builder.Build();

app.EnsureDatabaseCreated()
    .ConfigureMiddleware();

app.Run();