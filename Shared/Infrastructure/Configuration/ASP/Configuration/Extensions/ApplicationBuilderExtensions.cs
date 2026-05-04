using EcotrackPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace EcotrackPlatform.API.Shared.Infrastructure.Configuration.ASP.Configuration.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication EnsureDatabaseCreated(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }

        return app;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("AllowAllPolicy");

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