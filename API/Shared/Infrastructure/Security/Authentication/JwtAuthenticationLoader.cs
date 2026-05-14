using EcotrackPlatform.API.Shared.Infrastructure.Configuration.Utils;

namespace EcotrackPlatform.API.Shared.Infrastructure.Security.Authentication;

public static class JwtAuthConfigurationLoader
{
    private const string EnvSecretKey = "JWT_SECRET";
    private const string ConfigIssuer = "Jwt:Issuer";
    private const string ConfigAudience = "Jwt:Audience";

    private static string GetRequiredConfigurationVar(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        return string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException($"CRITICAL CONFIGURATION MISSING: Configuration variable '{key}' is not set.")
            : value;
    }

    public static JwtAuthConfiguration Load(IConfiguration configuration)
    {
        return new JwtAuthConfiguration(
            EnvVarUtils.GetRequiredEnvVar(EnvSecretKey),
            GetRequiredConfigurationVar(configuration, ConfigIssuer),
            GetRequiredConfigurationVar(configuration, ConfigAudience)
        );
    }
}