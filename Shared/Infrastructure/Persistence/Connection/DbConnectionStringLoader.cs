namespace EcotrackPlatform.API.Shared.Infrastructure.Persistence.Connection;

public static class DbConnectionStringLoader
{
    private const string EnvDbHost = "DB_HOST";
    private const string EnvDbPort = "DB_PORT";
    private const string EnvDbUser = "DB_USER";
    private const string EnvDbPassword = "DB_PASSWORD";
    private const string EnvDbName = "DB_NAME";

    private static string GetRequiredEnvVar(string variableName)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        return string.IsNullOrWhiteSpace(value) 
            ? throw new ArgumentNullException(variableName, $"...") 
            : value;
    }

    private static string GetOptionalEnvVar(string variableName, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    public static string GetConnectionString(bool isProduction)
    {
        var user = GetRequiredEnvVar(EnvDbUser);
        var password = GetRequiredEnvVar(EnvDbPassword);
        
        var host = GetOptionalEnvVar(EnvDbHost, "localhost");
        var port = GetOptionalEnvVar(EnvDbPort, "3306");
        var database = GetOptionalEnvVar(EnvDbName, "ecotrack"); 
        
        if (isProduction && (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || host == "127.0.0.1"))
        {
            throw new InvalidOperationException("CRITICAL: Database connection to localhost is strictly prohibited in the production environment.");
        }

        return $"server={host};port={port};user={user};password={password};database={database}";
    }
}