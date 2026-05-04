using EcotrackPlatform.API.Shared.Infrastructure.Configuration.Utils;

namespace EcotrackPlatform.API.Shared.Infrastructure.Persistence.Connection;

public static class DbConnectionStringLoader
{
    private const string EnvDbHost = "DB_HOST";
    private const string EnvDbPort = "DB_PORT";
    private const string EnvDbUser = "DB_USER";
    private const string EnvDbPassword = "DB_PASSWORD";
    private const string EnvDbName = "DB_NAME";

    public static string GetConnectionString(bool isProduction)
    {
        var user = EnvVarUtils.GetRequiredEnvVar(EnvDbUser);
        var password = EnvVarUtils.GetRequiredEnvVar(EnvDbPassword);
        
        var host =  EnvVarUtils.GetOptionalEnvVar(EnvDbHost, "localhost");
        var port =  EnvVarUtils.GetOptionalEnvVar(EnvDbPort, "3306");
        var database =  EnvVarUtils.GetOptionalEnvVar(EnvDbName, "ecotrack"); 
        
        if (isProduction && (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || host == "127.0.0.1"))
        {
            throw new InvalidOperationException("CRITICAL: Database connection to localhost is strictly prohibited in the production environment.");
        }

        return $"server={host};port={port};user={user};password={password};database={database}";
    }
}