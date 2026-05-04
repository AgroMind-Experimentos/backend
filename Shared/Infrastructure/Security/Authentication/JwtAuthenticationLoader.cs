using EcotrackPlatform.API.Shared.Infrastructure.Configuration.Utils;

namespace EcotrackPlatform.API.Shared.Infrastructure.Security.Authentication;

public static class JwtAuthConfigurationLoader
{
    private const string EnvSecretKey = "JWT_SECRET";
    private const string ConfigIssuer = "Jwt:Issuer";
    private const string ConfigAudience = "Jwt:Audience";
    private static readonly string[] AllConfigJsonVars = { ConfigIssuer, ConfigAudience };

    public static JwtAuthConfiguration Load(IConfiguration configuration)
    {
        var secret = EnvVarUtils.GetRequiredEnvVar(EnvSecretKey);

        var issuer = configuration[ConfigIssuer];
        var audience = configuration[ConfigAudience];
        
        foreach (var key in AllConfigJsonVars)
        {
            if (string.IsNullOrWhiteSpace(configuration[key]))
            {
                throw new InvalidOperationException($"CRITICAL CONFIGURATION MISSING: '{key}' is not set in configuration.");
            }
        }

        return new JwtAuthConfiguration(secret, issuer!, audience!);
    }
}