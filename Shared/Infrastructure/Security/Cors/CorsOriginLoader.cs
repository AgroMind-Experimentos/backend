using EcotrackPlatform.API.Shared.Infrastructure.Configuration.Utils;

namespace EcotrackPlatform.API.Shared.Infrastructure.Security.Cors;

public static class CorsOriginLoader
{
    private const string EnvFrontendUrl = "FRONTEND_URL";

    public static string[] GetAllowedOrigins(bool isProduction)
    {
        string originsStr;

        if (isProduction)
        {
            originsStr = EnvVarUtils.GetRequiredEnvVar(EnvFrontendUrl);

            if (originsStr.Contains("localhost", StringComparison.OrdinalIgnoreCase) || originsStr.Contains("127.0.0.1"))
            {
                throw new InvalidOperationException("CRITICAL: CORS pointing to localhost is strictly prohibited in the production environment.");
            }
        }
        else
        {
            originsStr = EnvVarUtils.GetOptionalEnvVar(EnvFrontendUrl, "http://localhost:5173");
        }

        return originsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public static string GetPolicyName(bool isProduction)
    {
        return isProduction ? "AllowWebPolicy" : "AllowDevPolicy";
    }
}