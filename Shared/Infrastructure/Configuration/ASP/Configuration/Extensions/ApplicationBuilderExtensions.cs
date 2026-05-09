using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using EcotrackPlatform.API.Shared.Infrastructure.Security.Cors;

namespace EcotrackPlatform.API.Shared.Infrastructure.Configuration.ASP.Configuration.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        return app;
    }


    public static WebApplication ConfigureMiddleware(this WebApplication app, IWebHostEnvironment environment)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseCors(CorsOriginLoader.GetPolicyName(environment.IsProduction()));

        if (app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}